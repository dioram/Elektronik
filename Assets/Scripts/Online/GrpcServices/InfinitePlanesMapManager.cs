using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class InfinitePlanesMapManager : MapManager<SlamInfinitePlane>
    {
        ICSConverter m_converter;

        public InfinitePlanesMapManager(IContainer<SlamInfinitePlane> map, ICSConverter converter) : base(map)
        {
            m_converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.InfinitePlanes)
            {
                return Handle(request.Action, request.ExtractInfinitePlanes(m_converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}