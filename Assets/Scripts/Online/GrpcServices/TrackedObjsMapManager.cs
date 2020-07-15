using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class TrackedObjsMapManager : ConnectableObjectsMapManager<TrackedObjPb>
    {
        private GameObjectsContainer<TrackedObjPb> m_gameObjects;

        public TrackedObjsMapManager(
            GameObjectsContainer<TrackedObjPb> gameObjects, 
            IConnectableObjectsContainer<TrackedObjPb> map) : base(map)
        {
            m_gameObjects = gameObjects;
        }

        private Task<ErrorStatusPb> UpdateTracks(PacketPb request, Task<ErrorStatusPb> status)
        {
            var status_ = status.Result;
            if (status_.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded &&
                request.Action == PacketPb.Types.ActionType.Update)
            {
                bool result = true;
                foreach (var o in request.TrackedObjs.Data)
                {
                    if (m_gameObjects.TryGet(o, out GameObject helmetGO))
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
                    status_.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown;
                    status_.Message = "[TrackedObjsMapManager.Handle] something went wrong when updating tracked objects";
                }
            }
            return status;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Debug.Log("[TrackedObjsMapManager.Handle]");
            if (request.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var status = Handle(request.Action, request.TrackedObjs.Data);
                status = UpdateTracks(request, status);
                status = HandleConnections(request, status);
                return status;
            }
            return base.Handle(request, context);
        }
    }
}
