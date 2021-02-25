using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Elektronik.ProtobufPlugin.Online.GrpcServices
{
    public class TrackedObjsMapManager : MapManager<SlamTrackedObject>
    {
        private readonly ICSConverter _converter;

        public TrackedObjsMapManager(IContainer<SlamTrackedObject> container, ICSConverter converter) : base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var objs = request.ExtractTrackedObjects(_converter).ToList();
                return base.Handle(request.Action, objs);
            }
            return base.Handle(request, context);
        }
    }
}
