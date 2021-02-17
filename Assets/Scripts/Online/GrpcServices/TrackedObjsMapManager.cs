using System;
using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class TrackedObjsMapManager : ConnectableObjectsMapManager<SlamTrackedObject>
    {
        public CSConverter Converter;

        private Task<ErrorStatusPb> UpdateTracks(PacketPb request, Task<ErrorStatusPb> status)
        {
            var container = (Container as ConnectableTrackedObjsContainer)?.TrackedObjsContainer;
            if (container == null) throw new InvalidCastException();
            
            var tmpStatus = status.Result;
            if (tmpStatus.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded &&
                request.Action == PacketPb.Types.ActionType.Update)
            {
                bool result = true;
                foreach (var o in request.TrackedObjs.Data)
                {
                    if (container.TryGet(o, out GameObject helmetGO))
                    {
                        MainThreadInvoker.Instance.Enqueue(() =>
                        {
                            var helmet = helmetGO.GetComponent<Helmet>();
                            helmet.IncrementTrack();
                        });
                    }
                    else
                    {
                        result = false;
                    }
                }
                if (!result)
                {
                    tmpStatus.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown;
                    tmpStatus.Message = "[TrackedObjsMapManager.Handle] something went wrong when updating tracked objects";
                }
            }
            return status;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[TrackedObjsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var status = Handle(request.Action, request.ExtractTrackedObjects(Converter).ToList());
                status = UpdateTracks(request, status);
                status = HandleConnections(request, status);
                return status;
            }
            return base.Handle(request, context);
        }
    }
}
