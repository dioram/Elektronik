using System;
using System.Threading.Tasks;
using Elektronik.Common.Renderers;
using Elektronik.ProtobufPlugin.Common;
using Grpc.Core;

namespace Elektronik.ProtobufPlugin.Online.GrpcServices
{
    public class ImageManager : ImageManagerPb.ImageManagerPbBase
    {
        private readonly CameraImageRenderer _renderer;

        public ImageManager(CameraImageRenderer renderer)
        {
            _renderer = renderer;
        }

        public override Task<ErrorStatusPb> Handle(ImagePacketPb request, ServerCallContext context)
        {
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                _renderer.Render(request.ImageData.ToByteArray());
            }
            catch (Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }

            return Task.FromResult(err);
        }
    }
}