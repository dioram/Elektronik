using Elektronik.Data;
using Elektronik.RosPlugin.Common.Containers;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public class ImageHandler: MessageHandler<Image, ImagePresenter>
    {
        public ImageHandler(ISourceTreeNode container, RosSocket socket, string topic) : base(container, socket, topic)
        {
        }

        protected override void Handle(Image message)
        {
            Container?.Present(ImageDataExt.FromImageMessage(message));
        }
    }
}