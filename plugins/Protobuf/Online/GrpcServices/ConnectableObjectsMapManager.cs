using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Debug = UnityEngine.Debug;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public abstract class ConnectableObjectsMapManager<T> : MapManager<T> where T : ICloudItem
    {
        private readonly IConnectableObjectsContainer<T> _connectableContainer;

        protected ConnectableObjectsMapManager(IConnectableObjectsContainer<T> container) : base(container)
        {
            _connectableContainer = container;
        }

        protected virtual Task<ErrorStatusPb> HandleConnections(PacketPb request, Task<ErrorStatusPb> baseStatus)
        {
            Debug.Log("[HandleConnections]");
            var timer = Stopwatch.StartNew();
            var status = baseStatus.Result;

            if (status.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded ||
                request.Action != PacketPb.Types.ActionType.Update)
            {
                timer.Stop();
                Debug.Log($"[HandleConnections] {DateTime.Now} " +
                          $"Elapsed time: {Timer.ElapsedMilliseconds} ms. " +
                          $"BaseStatus: {baseStatus.Result} " +
                          $"Action: {request.Action}");
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
            }
            timer.Stop();
            Debug.Log($"[HandleConnections] {DateTime.Now} " +
                      $"Elapsed time: {Timer.ElapsedMilliseconds} ms. " +
                      $"ErrorStatus: {status}");
            return Task.FromResult(status);
        }
    }
}