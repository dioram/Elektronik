#if !NO_ROS2DDS
using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros2.Online.Handlers;
using ImageHandler = Elektronik.RosPlugin.Ros2.Online.Handlers.ImageHandler;
using OdometryHandler = Elektronik.RosPlugin.Ros2.Online.Handlers.OdometryHandler;
using PointCloud2Handler = Elektronik.RosPlugin.Ros2.Online.Handlers.PointCloud2Handler;
using PoseStampedHandler = Elektronik.RosPlugin.Ros2.Online.Handlers.PoseStampedHandler;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2OnlineContainerTree : RosContainerTree
    {
        public Ros2OnlineContainerTree(Ros2Settings settings, string displayName) : base(displayName)
        {
            DisplayName = $"ROS2: {settings.DomainId}";
            _connector = new Connector(settings.DomainId, new UnityLogger());
            _topicsHandler = new TopicsHandler();
            _topicsHandler.OnNewTopic += AddTopic;
            _connector.SubscribeOnNewTopics(_topicsHandler);
        }
        
        public override void Dispose()
        {
            lock (this)
            {
                base.Dispose();
                _connector.UnsubscribeFromNewTopics(_topicsHandler);
                _topicsHandler.Dispose();
                foreach (var pair in _handlers)
                {
                    _connector.UnsubscribeFromMessages(pair.Key, pair.Value);
                    pair.Value.Dispose();
                }

                _handlers.Clear();
                _connector.Dispose();
            }
        }

        #region Protected

        protected override ISourceTree CreateContainer(string topicName, string topicType)
        {
            lock (this)
            {
                var container = (ISourceTree) Activator.CreateInstance(SupportedMessages[topicType].container,
                                                                       topicName.Split('/').Last());
                var handler = (MessageHandler) Activator.CreateInstance(SupportedMessages[topicType].handler, container);
                _connector.SubscribeOnMessages(MessageExt.GetDdsTopic(topicName), 
                                                MessageExt.GetDdsType(topicType),
                                                handler);
                _handlers.Add(topicName, handler);
                return container;
            }
        }

        #endregion

        #region Private

        private static readonly Dictionary<string, (Type container, Type handler)> SupportedMessages = new()
        {
            {"geometry_msgs/msg/PoseStamped", (typeof(TrackedObjectsContainer), typeof(PoseStampedHandler))},
            {"nav_msgs/msg/Odometry", (typeof(TrackedObjectsContainer), typeof(OdometryHandler))},
            {"sensor_msgs/msg/PointCloud2", (typeof(CloudContainer<SlamPoint>), typeof(PointCloud2Handler))},
            {"sensor_msgs/msg/Image", (typeof(ImagePresenter), typeof(ImageHandler))},
        };

        private readonly Dictionary<string, MessageHandler> _handlers = new();
        private readonly Connector _connector;
        private readonly TopicsHandler _topicsHandler;

        private void AddTopic(string topicName, string topicType)
        {
            if (!SupportedMessages.Keys.Contains(topicType)) return;
            if (ActualTopics.Contains((topicName, topicType))) return;

            ActualTopics.Add((topicName, topicType));
            RebuildTree();
        }

        #endregion
    }
}
#endif