using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Online.GrpcServices;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    class LinesMapManager : MapManager<SlamLine>
    {
        public LinesMapManager(IContainer<SlamLine> map) : base(map)
        {
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Lines)
            {
                var lines = request.Lines.Data.Select(p => (SlamLine)p).ToList();
                return Handle(request.Action, lines);
            }

            return base.Handle(request, context);
        }
    }
}
