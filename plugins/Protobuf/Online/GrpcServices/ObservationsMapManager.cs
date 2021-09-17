using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class ObservationsMapManager : ConnectableObjectsMapManager<SlamObservation, SlamObservationDiff>
    {
        public ObservationsMapManager(OnlineFrameBuffer buffer, IConnectableObjectsContainer<SlamObservation> container,
                                      ICSConverter? converter, ILogger logger)
            : base(buffer, container, converter, logger)
        {
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Observations) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var timestamp = DateTime.Now;
            var obs = request.ExtractObservations(Converter, Directory.GetCurrentDirectory());
            return HandleConnections(request, Handle(request.Action, obs, request.Special, timestamp), request.Special,
                                     timestamp);
        }
    }
}