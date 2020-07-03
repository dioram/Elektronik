using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class ObservationsMapManager : MapManager<SlamObservation>
    {
        public ObservationsMapManager(IContainer<SlamObservation> map) : base(map)
        {
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ObservationsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.ObservationsPacket)
            {
                var obs = request.ObservationsPacket.Observations.Select(p => (SlamObservation)p);
                return Handle(request.Action, obs);
            }

            return base.Handle(request, context);
        }
    }
}
