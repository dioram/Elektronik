﻿using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using System;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Online.GrpcServices
{
    public abstract class ConnectableObjectsMapManager<T> : MapManager<T> where T: ICloudItem
    {
        public ConnectableObjectsContainer<T> ObjectsContainer;

        #region Unity events

        protected virtual void Awake()
        {
            Container = ObjectsContainer;
        }

        #endregion

        protected virtual Task<ErrorStatusPb> HandleConnections(PacketPb request, Task<ErrorStatusPb> baseStatus)
        {
            var status = baseStatus.Result;

            if (status.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded ||
                request.Action != PacketPb.Types.ActionType.Update)
            {
                return Task.FromResult(status);
            }

            if (request.Connections != null && request.Connections.Data.Count != 0)
            {
                var connections = request.Connections.Data.Select(c => (c.Id1, c.Id2));
                try
                {
                    if (request.Connections.Action == PacketPb.Types.Connections.Types.Action.Add)
                        ObjectsContainer.AddConnections(connections);
                    if (request.Connections.Action == PacketPb.Types.Connections.Types.Action.Remove)
                        ObjectsContainer.RemoveConnections(connections);
                }
                catch (Exception e)
                {
                    status.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                    status.Message = e.Message;
                }
            }
            return Task.FromResult(status);
        }
    }
}
