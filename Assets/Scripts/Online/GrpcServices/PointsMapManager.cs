using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class PointsMapManager : ConnectableObjectsMapManager<SlamPoint>
    {
        public CSConverter Converter;
        
        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[PointsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Points)
            {
                var pts = request.ExtractPoints(Converter).ToList();
                return HandleConnections(request, Handle(request.Action, pts));
            }

            return base.Handle(request, context);
        }
    }
}
