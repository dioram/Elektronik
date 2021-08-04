#if !NO_ROS2DDS
using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.Containers;

namespace Elektronik.RosPlugin.Ros2.Online.Handlers
{
    public class ImageHandler : MessageHandler
    {
        private readonly ImagePresenter _presenter;

        public ImageHandler(ImagePresenter presenter)
        {
            _presenter = presenter;
        }

        public override void Handle(Ros2Message message)
        {
            var image = message.CastTo<ImageMessage>();
            if (image is null) return;
            _presenter.Present(new ImageData
            {
                Width = (int) image.width, 
                Height = (int) image.height,
                Encoding = ImageDataExt.GetTextureFormat(image.encoding),
                Data = image.data.ToArray()
            });
            message.Dispose();
        }
    }
}
#endif