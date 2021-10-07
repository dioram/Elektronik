using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Elektronik.Plugins.Common.FrameBuffers;
using Elektronik.Plugins.Common.Parsing;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.Presenters;
using Grpc.Core;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class ImageManager : MapsManagerPb.MapsManagerPbBase, IChainable<MapsManagerPb.MapsManagerPbBase>
    {
        private readonly ILogger _logger;
        private readonly ImagePresenter _presenter;
        private readonly OnlineFrameBuffer _buffer;
        private ImageCommand? _lastCommand;

        public ImageManager(OnlineFrameBuffer buffer, ImagePresenter presenter, ILogger logger)
        {
            _presenter = presenter;
            _logger = logger;
            _buffer = buffer;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Image)
            {
                Task<ErrorStatusPb> status;
                if (_link != null)
                {
                    status = _link.Handle(request, context);
                }
                else
                {
                    status = Task.FromResult(new ErrorStatusPb
                    {
                        ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Unknown,
                        Message = "Valid MapManager not found for this message"
                    });
                }

                return status;
            }

            var timer = Stopwatch.StartNew();
            var err = new ErrorStatusPb { ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded };
            try
            {
                var command = new ImageCommand(_presenter, request.ExtractImage(Directory.GetCurrentDirectory()),
                                               _lastCommand);
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

        private MapsManagerPb.MapsManagerPbBase? _link;

        public IChainable<MapsManagerPb.MapsManagerPbBase>? SetSuccessor(
            IChainable<MapsManagerPb.MapsManagerPbBase>? link)
        {
            _link = link as MapsManagerPb.MapsManagerPbBase;
            return link;
        }
    }
}