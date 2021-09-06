using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Elektronik.Commands;
using Elektronik.Commands.Generic;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.OnlineBuffered.GrpcServices
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
        protected readonly UpdatableFramesCollection<ICommand> Buffer;
        protected readonly ICSConverter? Converter;

        protected MapManager(UpdatableFramesCollection<ICommand> buffer, IContainer<TCloudItem> container,
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

        protected Task<ErrorStatusPb> Handle(PacketPb.Types.ActionType action, IList<TCloudItemDiff> data)
        {
            ErrorStatusPb errorStatus = new() { ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded };
            try
            {
                lock (Container)
                {
                    switch (action)
                    {
                    case PacketPb.Types.ActionType.Add:
                        Buffer.Add(new AddCommand<TCloudItem, TCloudItemDiff>(Container, data));
                        break;
                    case PacketPb.Types.ActionType.Update:
                        Buffer.Add(new UpdateCommand<TCloudItem, TCloudItemDiff>(Container, data));
                        break;
                    case PacketPb.Types.ActionType.Remove:
                        Buffer.Add(new RemoveCommand<TCloudItem, TCloudItemDiff>(Container, data));
                        break;
                    case PacketPb.Types.ActionType.Clear:
                        Buffer.Add(new ClearCommand<TCloudItem>(Container));
                        break;
                    }
                }
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