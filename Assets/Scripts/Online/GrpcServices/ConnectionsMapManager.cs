using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Online.GrpcServices;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Online.GrpcServices
{
    class ConnectionsMapManager : MapManager<SlamLine>
    {
        PacketPb.Types.ConnectionsPacket.Types.MapType m_mapType;
        public ConnectionsMapManager(IContainer<SlamLine> map, PacketPb.Types.ConnectionsPacket.Types.MapType mapType) : base(map)
        {
            m_mapType = mapType;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.ConnectionsPacket &&
                request.ConnectionsPacket.Map == m_mapType)
            {
                var lines = request.ConnectionsPacket.Connections.Select(p => (SlamLine)p);
                return Handle(request.Action, lines);
            }

            return base.Handle(request, context);
        }
    }
}
