using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.Settings;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagContainerTree : RosContainerTree
    {
        protected static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/PoseStamped", typeof(TrackedObjectsContainer)},
            {"nav_msgs/Odometry", typeof(TrackedObjectsContainer)},
            {"sensor_msgs/PointCloud2", typeof(CloudContainer<SlamPoint>)},
            // {"visualization_msgs/msg/MarkerArray", typeof(CloudContainer<SlamPoint>)},
        };
        
        public BagParser? Parser { get; private set; }
        
        public RosbagContainerTree(string displayName) : base(displayName)
        {
        }

        public override void Init(FileScaleSettingsBag settings)
        {
            Parser = new BagParser(settings.FilePath);
            ActualTopics = Parser.GetTopics()
                    .Where(t => SupportedMessages.ContainsKey(t.Type))
                    .Select(t => (t.Topic, t.Type))
                    .ToArray();
            base.Init(settings);
        }

        public override void Reset()
        {
            Parser?.Dispose();
            base.Reset();
        }

        #region Protected

        public override IContainerTree CreateContainer(string topicName, string topicType)
        {
            return (IContainerTree) Activator.CreateInstance(SupportedMessages[topicType],
                                                             topicName.Split('/').Last());
        }

        #endregion
    }
}