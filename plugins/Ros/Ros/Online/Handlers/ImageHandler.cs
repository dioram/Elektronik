using Elektronik.Containers;
using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public class ImageHandler: MessageHandler<Image, ImagePresenter>
    {
        public ImageHandler(ISourceTree container, RosSocket socket, string topic) : base(container, socket, topic)
        {
        }

        protected override void Handle(Image message)
        {
            Container?.Present(ImageDataExt.FromImageMessage(message));
        }
    }
}