using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common;
using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Plugins.Common.FrameBuffers;
using Elektronik.Protobuf.Data;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public abstract class ConnectableObjectsMapManager<TCloudItem, TCloudItemDiff>
            : MapManager<TCloudItem, TCloudItemDiff>
            where TCloudItem : struct, ICloudItem
            where TCloudItemDiff : ICloudItemDiff<TCloudItemDiff, TCloudItem>
    {
        private readonly IConnectableObjectsContainer<TCloudItem> _connectableContainer;

        protected ConnectableObjectsMapManager(IOnlineFrameBuffer buffer,
                                               IConnectableObjectsContainer<TCloudItem> container,
                                               ICSConverter? converter, ILogger logger)
                : base(buffer, container, converter, logger)
        {
            _connectableContainer = container;
        }

        protected Task<ErrorStatusPb> HandleConnections(PacketPb request, Task<ErrorStatusPb> baseStatus,
                                                        bool isKeyFrame, DateTime timestamp)
        {
            var timer = Stopwatch.StartNew();
            var status = baseStatus.Result;

            if (status.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded ||
                request.Action != PacketPb.Types.ActionType.Update)
            {
                timer.Stop();
                return Task.FromResult(status);
            }

            if (request.Connections != null && request.Connections.Data.Count != 0)
            {
                var connections = request.Connections.Data.Select(c => (c.Id1, c.Id2)).ToArray();
                try
                {
                    ICommand? command = request.Connections.Action switch
                    {
                        PacketPb.Types.Connections.Types.Action.Add => new AddConnectionsCommand<TCloudItem>(
                            _connectableContainer, connections),
                        PacketPb.Types.Connections.Types.Action.Remove => new RemoveConnectionsCommand<TCloudItem>(
                            _connectableContainer, connections),
                        _ => null,
                    };
                    if (command != null) Buffer.Add(command, timestamp, isKeyFrame);
                }
                catch (Exception e)
                {
                    status.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                    status.Message = e.Message;
                }

                timer.Stop();
                Logger.Info($"[HandleConnections] {DateTime.Now} " +
                            $"Elapsed time: {timer.ElapsedMilliseconds} ms. " +
                            $"ErrorStatus: {status}");
            }

            timer.Stop();
            return Task.FromResult(status);
        }
    }
}