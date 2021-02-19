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
        public CSConverter Converter;
        public SlamInfinitePlanesContainer PlanesContainer;

        #region Unity events

        protected virtual void Awake()
        {
            Container = PlanesContainer;
        }

        #endregion

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.InfinitePlanes)
            {
                return Handle(request.Action, request.ExtractInfinitePlanes(Converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}