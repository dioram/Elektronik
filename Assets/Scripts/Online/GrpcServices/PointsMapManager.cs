using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Converters;
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
    public class PointsMapManager : ConnectableObjectsMapManager<SlamPoint>
    {
        ICSConverter m_converter;

        public PointsMapManager(IConnectableObjectsContainer<SlamPoint> map, ICSConverter converter) : base(map)
        {
            m_converter = converter;
        }
        
        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[PointsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Points)
            {
                var pts = request.ExtractPoints(m_converter).ToList();
                return HandleConnections(request, Handle(request.Action, pts));
            }

            return base.Handle(request, context);
        }
    }
}
