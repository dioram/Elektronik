using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Elektronik.Plugins.Common.FrameBuffers;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.Presenters;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class ImageManager : ImageManagerPb.ImageManagerPbBase
    {
        private readonly ILogger _logger;
        private readonly RawImagePresenter _presenter;
        private readonly OnlineFrameBuffer _buffer;
        private ImageCommand? _lastCommand;

        public ImageManager(RawImagePresenter presenter, OnlineFrameBuffer buffer, ILogger logger)
        {
            _presenter = presenter;
            _logger = logger;
            _buffer = buffer;
        }

        public override Task<ErrorStatusPb> Handle(ImagePacketPb request, ServerCallContext context)
        {
            var timer = Stopwatch.StartNew();
            var err = new ErrorStatusPb
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                var command = new ImageCommand(_presenter, request.ImageData.ToByteArray(), _lastCommand);
                _buffer.Add(command, DateTime.Now, false);
                _lastCommand = command;
            }
            catch (Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }

            timer.Stop();
            _logger.Info($"[{GetType().Name}.Handle] Elapsed time: {timer.ElapsedMilliseconds} ms. ErrorStatus: {err}");

            return Task.FromResult(err);
        }
    }
}