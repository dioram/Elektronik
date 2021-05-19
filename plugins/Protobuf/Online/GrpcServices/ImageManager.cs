using System;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.Presenters;
using Grpc.Core;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class ImageManager : ImageManagerPb.ImageManagerPbBase
    {
        private readonly RawImagePresenter _presenter;

        public ImageManager(RawImagePresenter presenter)
        {
            _presenter = presenter;
        }

        public override Task<ErrorStatusPb> Handle(ImagePacketPb request, ServerCallContext context)
        {
            var timer = Stopwatch.StartNew();
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                _presenter.Present(request.ImageData.ToByteArray());
            }
            catch (Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }

            timer.Stop();
            try
            {
                UnityEngine.Debug.Log($"[{GetType().Name}.Handle] Elapsed time: {timer.ElapsedMilliseconds} ms. " +
                                      $"ErrorStatus: {err}");
            }
            catch (SecurityException e)
            {
                // This will be thrown if code was called in test environment.
                // Just ignore it
            }

            return Task.FromResult(err);
        }
    }
}