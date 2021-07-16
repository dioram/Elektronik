using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class TrackedObjsMapManager : MapManager<SlamTrackedObject, SlamTrackedObjectDiff>
    {
        private readonly ICSConverter _converter;

        public TrackedObjsMapManager(IContainer<SlamTrackedObject> container, ICSConverter converter) : base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.TrackedObjs) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var objs = request.ExtractTrackedObjects(_converter).ToList();
            return base.Handle(request.Action, objs);
        }
    }
}