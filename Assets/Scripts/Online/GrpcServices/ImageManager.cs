using System;
using System.Threading.Tasks;
using Elektronik.Common.Cameras;
using Elektronik.Common.Data.Pb;
using Grpc.Core;

namespace Elektronik.Online.GrpcServices
{
    public class ImageManager : ImageManagerPb.ImageManagerPbBase
    {
        private CameraImageRenderer m_renderer;
        public ImageManager(CameraImageRenderer renderer)
        {
            m_renderer = renderer;
        }

        public override Task<ErrorStatusPb> Handle(ImagePacketPb request, ServerCallContext context)
        {
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                m_renderer.DrawImage(request.Width, request.Height, request.ImageData.ToByteArray());
            }
            catch(Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }
            return Task.FromResult(err);
        }
    }
}