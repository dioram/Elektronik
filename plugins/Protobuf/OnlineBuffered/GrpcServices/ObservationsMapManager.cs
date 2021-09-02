using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Commands;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.OnlineBuffered.GrpcServices
{
    public class ObservationsMapManager : ConnectableObjectsMapManager<SlamObservation, SlamObservationDiff>
    {
        public ObservationsMapManager(UpdatableFramesCollection<ICommand> buffer,
                                      IConnectableObjectsContainer<SlamObservation> container, ICSConverter? converter,
                                      ILogger logger)
                : base(buffer, container, converter, logger)
        { }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Observations) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var obs = request.ExtractObservations(Converter, Directory.GetCurrentDirectory());
            return HandleConnections(request, Handle(request.Action, obs));
        }
    }
}