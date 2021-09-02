using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class PointsMapManager : ConnectableObjectsMapManager<SlamPoint, SlamPointDiff>
    {
        private readonly ICSConverter? _converter;

        public PointsMapManager(IConnectableObjectsContainer<SlamPoint> container, ICSConverter? converter,
                                ILogger logger)
                : base(container, logger)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Points) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            return HandleConnections(request, Handle(request.Action, request.ExtractPoints(_converter)));
        }
    }
}