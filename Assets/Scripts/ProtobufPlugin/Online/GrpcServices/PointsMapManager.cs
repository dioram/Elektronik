using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Online.GrpcServices
{
    public class PointsMapManager : ConnectableObjectsMapManager<SlamPoint>
    {
        private readonly ICSConverter _converter;

        public PointsMapManager(IConnectableObjectsContainer<SlamPoint> container, ICSConverter converter) : base(container)
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
