using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Debug = UnityEngine.Debug;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    class LinesMapManager : MapManager<SlamLine>
    {
        private readonly ICSConverter _converter;

        public LinesMapManager(IContainer<SlamLine> container, ICSConverter converter) : base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase == PacketPb.DataOneofCase.Lines)
            {
                Debug.Log("[LinesMapManager.Handle]");
                Timer = Stopwatch.StartNew();
                return Handle(request.Action, request.ExtractLines(_converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}
