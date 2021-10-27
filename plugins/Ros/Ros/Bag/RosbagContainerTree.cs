using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros.Bag.Parsers;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagContainerTree : RosContainerTree
    {
        private static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/PoseStamped", typeof(TrackedCloudObjectsContainer)},
            {"nav_msgs/Odometry", typeof(TrackedCloudObjectsContainer)},
            {"sensor_msgs/PointCloud2", typeof(CloudContainer<SlamPoint>)},
            {"sensor_msgs/Image", typeof(ImagePresenter)},
            {"std_msgs/String", typeof(StringPresenter)},
            {"*", typeof(UnknownTypePresenter)},
        };

        public BagParser Parser { get; }

        public RosbagContainerTree(RosbagSettings settings, string displayName) : base(displayName)
        {
            DisplayName = settings.PathToBag.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag: /";
            Parser = new BagParser(settings.PathToBag);
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

        protected override IDataSource CreateContainer(string topicName, string topicType)
        {
            if (SupportedMessages.ContainsKey(topicType))
            {
                return (IDataSource) Activator.CreateInstance(SupportedMessages[topicType],
                                                              topicName.Split('/').Last());
            }
            
            return (IDataSource) Activator.CreateInstance(SupportedMessages["*"],
                                                          topicName.Split('/').Last());
        }

        #endregion
    }
}