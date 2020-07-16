using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
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
        ICSConverter m_converter;

        public LinesMapManager(IContainer<SlamLine> map, ICSConverter converter) : base(map)
        {
            m_converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[ConnectionsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.Lines)
            {
                return Handle(request.Action, request.ExtractLines(m_converter).ToList());
            }

            return base.Handle(request, context);
        }
    }
}
