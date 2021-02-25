using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Online.GrpcServices
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
