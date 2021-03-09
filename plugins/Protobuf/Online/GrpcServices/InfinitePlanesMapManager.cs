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
    public class InfinitePlanesMapManager : MapManager<SlamInfinitePlane>
    {
        private readonly ICSConverter _converter;

        public InfinitePlanesMapManager(IContainer<SlamInfinitePlane> container, ICSConverter converter) 
                : base(container)
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