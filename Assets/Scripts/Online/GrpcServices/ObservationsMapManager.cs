using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class ObservationsMapManager : ConnectableObjectsMapManager<SlamObservation>
    {
        public CSConverter Converter;

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ObservationsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Observations)
            {
                var obs = request.ExtractObservations(Converter).ToList();
                return HandleConnections(request, Handle(request.Action, obs));
            }

            return base.Handle(request, context);
        }
    }
}
