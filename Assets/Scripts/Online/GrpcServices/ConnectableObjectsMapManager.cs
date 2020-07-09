using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Online.GrpcServices
{
    public abstract class ConnectableObjectsMapManager<T> : MapManager<T>
    {
        private IConnectableObjectsContainer<T> m_map;
        public ConnectableObjectsMapManager(IConnectableObjectsContainer<T> map) : base(map)
        {
            m_map = map;
        }

        protected virtual Task<ErrorStatusPb> HandleConnections(PacketPb request, Task<ErrorStatusPb> baseStatus)
        {
            var status = baseStatus.Result;

            if (status.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded ||
                request.Action != PacketPb.Types.ActionType.Update)
            {
                return Task.FromResult(status);
            }

            if (request.Connections != null && request.Connections.Data.Count != 0)
            {
                var connections_ = request.Connections.Data.Select(c => (c.Id1, c.Id2));
                bool result = false;
                if (request.Connections.Action == PacketPb.Types.Connections.Types.Action.Add)
                    result = m_map.AddConnections(connections_);
                if (request.Connections.Action == PacketPb.Types.Connections.Types.Action.Remove)
                    result = m_map.RemoveConnections(connections_);
                if (!result)
                {
                    status.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown;
                    status.Message = "Something went wrong while update connections";
                }
            }
            return Task.FromResult(status);
        }
    }
}
