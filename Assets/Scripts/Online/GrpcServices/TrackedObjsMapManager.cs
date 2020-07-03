using Elektronik.Common.Containers;
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
    public class TrackedObjsMapManager : MapManager<TrackedObjPb>
    {
        
        public TrackedObjsMapManager(IContainer<TrackedObjPb> map) : base(map)
        {
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[TrackedObjsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.TrackedObjsPacket)
            {
                return Handle(request.Action, request.TrackedObjsPacket.TrackedObjs);
            }
            return base.Handle(request, context);
        }
    }
}
