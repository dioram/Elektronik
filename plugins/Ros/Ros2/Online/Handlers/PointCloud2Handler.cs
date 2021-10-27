#if !NO_ROS2DDS
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.RosPlugin.Ros2.Online.Handlers
{
    public class PointCloud2Handler : MessageHandler
    {
        private readonly IContainer<SlamPoint> _container;

        public PointCloud2Handler(IContainer<SlamPoint> container)
        {
            _container = container;
        }

        public override void Handle(Ros2Message message)
        {
            var cloud = message.CastTo<PointCloud2Message>();
            if (cloud == null) return;
            _container.Clear();
            _container.AddRange(cloud.ToSlamPoints());
        }
    }
}
#endif