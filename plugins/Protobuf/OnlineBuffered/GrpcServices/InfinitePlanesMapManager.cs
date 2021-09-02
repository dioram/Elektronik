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
    public class InfinitePlanesMapManager : MapManager<SlamInfinitePlane, SlamInfinitePlaneDiff>
    {
        public InfinitePlanesMapManager(UpdatableFramesCollection<ICommand> buffer,
                                        IContainer<SlamInfinitePlane> container, ICSConverter? converter,
                                        ILogger logger)
                : base(buffer, container, converter, logger)
        { }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.InfinitePlanes) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            return Handle(request.Action, request.ExtractInfinitePlanes(Converter));
        }
    }
}