﻿using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Commands;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.OnlineBuffered.GrpcServices
{
    public class TrackedObjsMapManager : MapManager<SlamTrackedObject, SlamTrackedObjectDiff>
    {
        public TrackedObjsMapManager(UpdatableFramesCollection<ICommand> buffer,
                                     IContainer<SlamTrackedObject> container, ICSConverter? converter,
                                     ILogger logger)
                : base(buffer, container, converter, logger)
        { }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.TrackedObjs) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            return base.Handle(request.Action, request.ExtractTrackedObjects(Converter));
        }
    }
}