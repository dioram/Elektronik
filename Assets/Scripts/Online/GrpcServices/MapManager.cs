﻿using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Elektronik.Online.GrpcServices
{
    using UnityDebug = UnityEngine.Debug;
    public abstract class MapManager<T> : MapsManagerPb.MapsManagerPbBase, IChainable<MapsManagerPb.MapsManagerPbBase>
    {
        MapsManagerPb.MapsManagerPbBase _link;
        public IChainable<MapsManagerPb.MapsManagerPbBase> SetSuccessor(IChainable<MapsManagerPb.MapsManagerPbBase> link)
        {
            UnityDebug.Assert(link != this, "[DataParser.SetSuccessor] Cyclic reference!");
            _link = link as MapsManagerPb.MapsManagerPbBase;
            return link;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Task<ErrorStatusPb> status;
            if (_link != null)
                status = _link.Handle(request, context);
            else
                status = Task.FromResult(new ErrorStatusPb() 
                {
                    ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown, 
                    Message = "Valid MapManager not found for this message" 
                });
            return status;
        }
        
        protected IContainer<T> Container;

        protected Task<ErrorStatusPb> Handle(PacketPb.Types.ActionType action, IList<T> data)
        {
            var readOnlyData = new ReadOnlyCollection<T>(data);
            var timer = new Stopwatch();
            timer.Restart();
            ErrorStatusPb errorStatus = new ErrorStatusPb() { ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded };
            try
            {
                lock(Container)
                {
                    switch (action)
                    {
                        case PacketPb.Types.ActionType.Add:
                            Container.AddRange(readOnlyData);
                            break;
                        case PacketPb.Types.ActionType.Update:
                            Container.UpdateItems(readOnlyData);
                            break;
                        case PacketPb.Types.ActionType.Remove:
                            Container.Remove(readOnlyData);
                            break;
                        case PacketPb.Types.ActionType.Clear:
                            Container.Clear();
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                errorStatus.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                errorStatus.Message = err.Message;
            }
            timer.Stop();
            UnityDebug.Log($"[MapManager.Handle] Elapsed time: {timer.ElapsedMilliseconds} ms");
            return Task.FromResult(errorStatus);
        }
    }
}