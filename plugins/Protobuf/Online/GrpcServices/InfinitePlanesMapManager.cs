using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Debug = UnityEngine.Debug;

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
            if (request.DataCase == PacketPb.DataOneofCase.InfinitePlanes)
            {
                Debug.Log("[InfinitePlanesMapManager.Handle]");
                Timer = Stopwatch.StartNew();
                return Handle(request.Action, request.ExtractInfinitePlanes(_converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}