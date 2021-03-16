using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    class LinesMapManager : MapManager<SlamLine>
    {
        public CSConverter Converter;
        
        public SlamLinesContainer LinesContainer;

        #region Unity events

        protected virtual void Awake()
        {
            Container = LinesContainer;
        }

        #endregion

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Lines)
            {
                return Handle(request.Action, request.ExtractLines(Converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}
