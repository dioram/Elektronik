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
    public class LinesMapManager : MapManager<SlamLine, SlamLineDiff>
    {
        private readonly ICSConverter? _converter;

        public LinesMapManager(IContainer<SlamLine> container, ICSConverter? converter, ILogger logger) 
                : base(container, logger)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Lines) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            return Handle(request.Action, request.ExtractLines(_converter).ToList());
        }
    }
}