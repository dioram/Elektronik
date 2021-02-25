using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Elektronik.Online.GrpcServices
{
    public class TrackedObjsMapManager : MapManager<SlamTrackedObject>
    {
        public TrackedObjectsContainer ObjectsContainer;
        public CSConverter Converter;

        public void Awake()
        {
            Container = ObjectsContainer;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var objs = request.ExtractTrackedObjects(Converter).ToList();
                return base.Handle(request.Action, objs);
            }
            return base.Handle(request, context);
        }
    }
}
