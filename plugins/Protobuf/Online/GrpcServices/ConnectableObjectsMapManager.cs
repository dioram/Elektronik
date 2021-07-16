using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public abstract class ConnectableObjectsMapManager<T, U> : MapManager<T, U> 
            where T : struct, ICloudItem
            where U : struct, ICloudItemDiff<T>
    {
        private readonly IConnectableObjectsContainer<T> _connectableContainer;

        protected ConnectableObjectsMapManager(IConnectableObjectsContainer<T> container) : base(container)
        {
            _connectableContainer = container;
        }

        protected virtual Task<ErrorStatusPb> HandleConnections(PacketPb request, Task<ErrorStatusPb> baseStatus)
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
                var connections = request.Connections.Data.Select(c => (c.Id1, c.Id2));
                try
                {
                    switch (request.Connections.Action)
                    {
                    case PacketPb.Types.Connections.Types.Action.Add:
                        _connectableContainer.AddConnections(connections);
                        break;
                    case PacketPb.Types.Connections.Types.Action.Remove:
                        _connectableContainer.RemoveConnections(connections);
                        break;
                    }
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