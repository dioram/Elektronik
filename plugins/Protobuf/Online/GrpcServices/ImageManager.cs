using System;
using System.Threading.Tasks;
using Elektronik.Protobuf.Data;
using Elektronik.Renderers;
using Grpc.Core;

namespace Elektronik.Protobuf.Online.GrpcServices
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