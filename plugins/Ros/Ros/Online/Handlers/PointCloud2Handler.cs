using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public class PointCloud2Handler : MessageHandler<PointCloud2, IContainer<SlamPoint>>
    {
        public PointCloud2Handler(ISourceTree container, RosSocket socket, string topic)
                : base(container, socket, topic)
        {
        }

        protected override void Handle(PointCloud2 message)
        {
            Container?.Clear();
            Container?.AddRange(message.ToSlamPoints());
        }
    }
}