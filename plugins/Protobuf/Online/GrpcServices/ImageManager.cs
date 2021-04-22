using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.Presenters;
using Grpc.Core;
using Debug = UnityEngine.Debug;

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
            Debug.Log("[ImageMapManager.Handle]");
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
            Debug.Log($"[MapManager.Handle] Elapsed time: {timer.ElapsedMilliseconds} ms. " +
                      $"ErrorStatus: {err}");
            return Task.FromResult(err);
        }
    }
}