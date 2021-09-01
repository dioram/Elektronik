using System.Diagnostics;
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
    public class PointsMapManager : ConnectableObjectsMapManager<SlamPoint, SlamPointDiff>
    {
        public PointsMapManager(UpdatableFramesCollection<ICommand> buffer,
                                IConnectableObjectsContainer<SlamPoint> container, ICSConverter? converter,
                                ILogger logger)
                : base(buffer, container, converter, logger)
        { }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Points) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var pts = request.ExtractPoints(Converter).ToList();
            return HandleConnections(request, Handle(request.Action, pts));
        }
    }
}