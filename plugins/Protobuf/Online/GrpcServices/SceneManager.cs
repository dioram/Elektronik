using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Protobuf.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class SceneManager : SceneManagerPb.SceneManagerPbBase
    {
        public ILogger Logger = new UnityLogger();
        private readonly ISourceTree _container;

        public SceneManager(ISourceTree container)
        {
            _container = container;
        }

        public override Task<ErrorStatusPb> Clear(Empty request, ServerCallContext context)
        {
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
            Logger.Info($"[{GetType().Name}.Handle] Elapsed time: {timer.ElapsedMilliseconds} ms. ErrorStatus: {err.Message}");

            return Task.FromResult(err);
        }
    }
}