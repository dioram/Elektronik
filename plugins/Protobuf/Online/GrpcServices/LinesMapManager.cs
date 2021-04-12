using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Data.Pb;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Grpc.Core;
using UnityEngine;

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
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Lines)
            {
                return Handle(request.Action, request.ExtractLines(_converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}
