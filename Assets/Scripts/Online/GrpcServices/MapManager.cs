using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    using UnityDebug = UnityEngine.Debug;
    public abstract class MapManager<T> : MapsManagerPb.MapsManagerPbBase, IChainable<MapsManagerPb.MapsManagerPbBase>
    {
        MapsManagerPb.MapsManagerPbBase m_link;
        public IChainable<MapsManagerPb.MapsManagerPbBase> SetSuccessor(IChainable<MapsManagerPb.MapsManagerPbBase> link)
        {
            UnityDebug.Assert(link != this, "[DataParser.SetSuccessor] Cyclic reference!");
            m_link = link as MapsManagerPb.MapsManagerPbBase;
            return link;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            Task<ErrorStatusPb> status;
            if (m_link != null)
                status = m_link.Handle(request, context);
            else
                status = Task.FromResult(new ErrorStatusPb() 
                {
                    ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown, 
                    Message = "Valid MapManager not found for this message" 
                });
            return status;
        }
            

        IContainer<T> m_map;

        protected MapManager(IContainer<T> map)
        {
            m_map = map;
        }

        protected Task<ErrorStatusPb> Handle(PacketPb.Types.ActionType action, IList<T> data)
        {
            var readOnlyData = new ReadOnlyCollection<T>(data);
            var timer = new Stopwatch();
            timer.Restart();
            ErrorStatusPb errorStatus = new ErrorStatusPb() { ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded };
            try
            {
                lock(m_map)
                {
                    switch (action)
                    {
                        case PacketPb.Types.ActionType.Add:
                            m_map.Add(readOnlyData);
                            break;
                        case PacketPb.Types.ActionType.Update:
                            m_map.Update(readOnlyData);
                            break;
                        case PacketPb.Types.ActionType.Remove:
                            m_map.Remove(readOnlyData);
                            break;
                        case PacketPb.Types.ActionType.Clear:
                            m_map.Clear();
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
