using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros.Bag.Parsers;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagContainerTree : RosContainerTree
    {
        private static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/PoseStamped", typeof(TrackedObjectsContainer)},
            {"nav_msgs/Odometry", typeof(TrackedObjectsContainer)},
            {"sensor_msgs/PointCloud2", typeof(CloudContainer<SlamPoint>)},
            {"sensor_msgs/Image", typeof(ImagePresenter)},
            {"std_msgs/String", typeof(StringPresenter)},
            {"*", typeof(UnknownTypePresenter)},
        };

        public BagParser Parser { get; }

        public RosbagContainerTree(RosbagSettings settings, string displayName) : base(displayName)
        {
            DisplayName = settings.FilePath.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag: /";
            Parser = new BagParser(settings.FilePath);
            ActualTopics = Parser.GetTopics()
                    .Where(t => SupportedMessages.ContainsKey(t.Type) || UnknownTypePresenter.CanParseTopic(t.Type))
                    .Select(t => (t.Topic, t.Type))
                    .ToList();
            RebuildTree();
        }

        public override void Dispose()
        {
            Parser.Dispose();
            base.Dispose();
        }

        #region Protected

        protected override ISourceTree CreateContainer(string topicName, string topicType)
        {
            if (SupportedMessages.ContainsKey(topicType))
            {
                return (ISourceTree) Activator.CreateInstance(SupportedMessages[topicType],
                                                              topicName.Split('/').Last());
            }
            
            return (ISourceTree) Activator.CreateInstance(SupportedMessages["*"],
                                                          topicName.Split('/').Last());
        }

        #endregion
    }
}