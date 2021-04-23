﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Protobuf.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Debug = UnityEngine.Debug;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class SceneManager : SceneManagerPb.SceneManagerPbBase
    {
        private readonly ISourceTree _container;

        public SceneManager(ISourceTree container)
        {
            _container = container;
        }

        public override Task<ErrorStatusPb> Clear(Empty request, ServerCallContext context)
        {
            Debug.Log("[SceneManager.Handle]");
            var timer = Stopwatch.StartNew();
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                foreach (var child in _container.Children)
                {
                    child.Clear();
                }
            }
            catch (Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }
            timer.Stop();
            Debug.Log($"[MapManager.Handle] Elapsed time: {timer.ElapsedMilliseconds} ms." +
                      $"ErrorStatus: {err.Message}");
            return Task.FromResult(err);
        }
    }
}
