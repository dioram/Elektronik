using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Online.GrpcServices
{
    public class InfinitePlanesMapManager : MapManager<SlamInfinitePlane>
    {
        private readonly ICSConverter _converter;

        public InfinitePlanesMapManager(IContainer<SlamInfinitePlane> container, ICSConverter converter) : base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.InfinitePlanes)
            {
                return Handle(request.Action, request.ExtractInfinitePlanes(_converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}