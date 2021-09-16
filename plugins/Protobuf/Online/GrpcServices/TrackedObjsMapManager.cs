using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class TrackedObjsMapManager : MapManager<SlamTrackedObject, SlamTrackedObjectDiff>
    {
        public TrackedObjsMapManager(OnlineFrameBuffer buffer, IContainer<SlamTrackedObject> container,
                                     ICSConverter? converter, ILogger logger)
                : base(buffer, container, converter, logger)
        {
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.TrackedObjs) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var timestamp = DateTime.Now;
            return base.Handle(request.Action, request.ExtractTrackedObjects(Converter), request.Special, timestamp);
        }
    }
}