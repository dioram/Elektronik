using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Data.Pb;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.Protobuf.Online.GrpcServices
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
                var obs = request.ExtractObservations(_converter, Directory.GetCurrentDirectory()).ToList();
                return HandleConnections(request, Handle(request.Action, obs));
            }

            return base.Handle(request, context);
        }
    }
}