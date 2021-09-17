using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Plugins.Common.FrameBuffers;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    /// <summary> 
    /// Base class for handle data in online mode (buffered). Used in pattern "Chain of responsibility".
    /// </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    /// <typeparam name="TCloudItemDiff"></typeparam>
    public abstract class MapManager<TCloudItem, TCloudItemDiff>
            : MapsManagerPb.MapsManagerPbBase, IChainable<MapsManagerPb.MapsManagerPbBase>
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : struct, ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        protected readonly ILogger Logger;
        protected readonly OnlineFrameBuffer Buffer;
        protected readonly ICSConverter? Converter;

        protected MapManager(OnlineFrameBuffer buffer, IContainer<TCloudItem> container,
                             ICSConverter? converter, ILogger logger)
        {
            Container = container;
            Logger = logger;
            Converter = converter;
            Buffer = buffer;
        }

        MapsManagerPb.MapsManagerPbBase? _link;

        public IChainable<MapsManagerPb.MapsManagerPbBase> SetSuccessor(
            IChainable<MapsManagerPb.MapsManagerPbBase> link)
        {
            _link = link as MapsManagerPb.MapsManagerPbBase;
            return link;
        }

        /// <summary> Handles gRPC request. </summary>
        /// <param name="request"> Packet to handle. </param>
        /// <param name="context"> Server call context </param>
        /// <returns> Async error status </returns>
        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Task<ErrorStatusPb> status;
            if (_link != null)
                status = _link.Handle(request, context);
            else
                status = Task.FromResult(new ErrorStatusPb()
                {
                    ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown,
                    Message = "Valid MapManager not found for this message"
                });
            return status;
        }

        protected readonly IContainer<TCloudItem> Container;
        protected Stopwatch? Timer;

        protected Task<ErrorStatusPb> Handle(PacketPb.Types.ActionType action, TCloudItemDiff[] data,
                                             bool isKeyFrame, DateTime timestamp)
        {
            ErrorStatusPb errorStatus = new() { ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded };
            try
            {
                ICommand command;
                lock (Container)
                {
                    command = action switch
                    {
                        PacketPb.Types.ActionType.Add => new AddCommand<TCloudItem, TCloudItemDiff>(Container, data),
                        PacketPb.Types.ActionType.Update => new UpdateCommand<TCloudItem, TCloudItemDiff>(
                            Container, data),
                        PacketPb.Types.ActionType.Remove => new RemoveCommand<TCloudItem, TCloudItemDiff>(
                            Container, data),
                        PacketPb.Types.ActionType.Clear => new ClearCommand<TCloudItem>(Container),
                        _ => throw new ArgumentOutOfRangeException(nameof(action), "Unknown action type")
                    };
                }

                Buffer.Add(command, timestamp, isKeyFrame);
            }
            catch (Exception err)
            {
                errorStatus.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                errorStatus.Message = err.Message;
            }

            Timer?.Stop();
            Logger.Info($"[{GetType().Name}.Handle] {DateTime.Now} " +
                        $"Elapsed time: {Timer?.ElapsedMilliseconds} ms. " +
                        $"Error status: {errorStatus}");

            return Task.FromResult(errorStatus);
        }
    }
}