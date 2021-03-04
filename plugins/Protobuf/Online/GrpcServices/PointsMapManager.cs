using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class PointsMapManager : ConnectableObjectsMapManager<SlamPoint>
    {
        private readonly ICSConverter _converter;

        public PointsMapManager(IConnectableObjectsContainer<SlamPoint> container, ICSConverter converter)
                : base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[PointsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Points)
            {
                var pts = request.ExtractPoints(_converter).ToList();
                return HandleConnections(request, Handle(request.Action, pts));
            }

            return base.Handle(request, context);
        }
    }
}