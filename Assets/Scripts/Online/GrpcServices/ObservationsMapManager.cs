using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
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
    public class ObservationsMapManager : ConnectableObjectsMapManager<SlamObservation>
    {
        ICSConverter m_converter;

        public ObservationsMapManager(IConnectableObjectsContainer<SlamObservation> map, ICSConverter converter) : base(map)
        {
            m_converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ObservationsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Observations)
            {
                var obs = request.ExtractObservations(m_converter).ToList();
                return HandleConnections(request, Handle(request.Action, obs));
            }

            return base.Handle(request, context);
        }
    }
}
