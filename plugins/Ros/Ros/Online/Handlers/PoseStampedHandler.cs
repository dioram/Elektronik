using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public class PoseStampedHandler : MessageHandler<PoseStamped, IContainer<SlamTrackedObject>>
    {
        public PoseStampedHandler(ISourceTree container, RosSocket socket, string topic) : base(container, socket, topic)
        {
        }
        
        protected override void Handle(PoseStamped message)
        {
            if (Container?.Count == 0) Container.Add(message.GetPose()!.ToTracked());
            else Container?.Update(message.GetPose()!.ToTracked());
        }
    }
}