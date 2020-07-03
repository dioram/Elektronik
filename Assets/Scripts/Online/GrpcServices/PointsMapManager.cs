using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class PointsMapManager : MapManager<SlamPoint>
    {
        public PointsMapManager(IContainer<SlamPoint> map) : base(map)
        {
        }
        
        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[PointsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.PointsPacket)
            {
                var pts = request.PointsPacket.Points.Select(p => (SlamPoint)p);
                return Handle(request.Action, pts);
            }

            return base.Handle(request, context);
        }
    }
}
