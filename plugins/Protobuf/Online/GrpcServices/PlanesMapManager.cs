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
    public class PlanesMapManager : MapManager<SlamPlane, SlamPlaneDiff>
    {
        public PlanesMapManager(OnlineFrameBuffer buffer, IContainer<SlamPlane> container,
                                        ICSConverter? converter, ILogger logger)
            : base(buffer, container, converter, logger)
        {
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Planes) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var timestamp = DateTime.Now;
            return Handle(request.Action, request.ExtractPlanes(Converter), request.Special, timestamp);
        }
    }
}