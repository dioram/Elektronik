using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Online.GrpcServices
{
    public class ObservationsMapManager : ConnectableObjectsMapManager<SlamObservation>
    {
        private readonly ICSConverter _converter;

        public ObservationsMapManager(IConnectableObjectsContainer<SlamObservation> container, ICSConverter converter) :
                base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ObservationsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Observations)
            {
                var obs = request.ExtractObservations(_converter).ToList();
                return HandleConnections(request, Handle(request.Action, obs));
            }

            return base.Handle(request, context);
        }
    }
}