using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros2.Online.Handlers;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2OnlineContainerTree : RosContainerTree
    {
        public Ros2OnlineContainerTree(string displayName) : base(displayName)
        {
            _logger = new UnityLogger();
        }

        public void Init(Ros2Settings settings)
        {
            lock (this)
            {
                DisplayName = $"ROS2: {settings.DomainId}";
                _connector = new Connector(settings.DomainId, _logger);
                _topicsHandler = new TopicsHandler();
                _topicsHandler.OnNewTopic += AddTopic;
                _connector.SubscribeOnNewTopics(_topicsHandler);
            }
        }

        public override void Reset()
        {
            lock (this)
            {
                base.Reset();
                _connector?.UnsubscribeFromNewTopics(_topicsHandler);
                _topicsHandler?.Dispose();
                _topicsHandler = null;
                foreach (var pair in _handlers)
                {
                    _connector?.UnsubscribeFromMessages(pair.Key, pair.Value);
                    pair.Value.Dispose();
                }

                _handlers.Clear();
                _connector?.Dispose();
                _connector = null;
            }
        }

        #region Protected

        protected override IContainerTree CreateContainer(string topicName, string topicType)
        {
            lock (this)
            {
                var container = (IContainerTree) Activator.CreateInstance(SupportedMessages[topicType].container,
                                                                          topicName.Split('/').Last());
                var handler = (MessageHandler) Activator.CreateInstance(SupportedMessages[topicType].handler, container);
                _connector?.SubscribeOnMessages(MessageExt.GetDdsTopic(topicName), 
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
        };

        private readonly Dictionary<string, MessageHandler> _handlers = new();
        private Connector? _connector;
        private TopicsHandler? _topicsHandler;
        private readonly UnityLogger _logger;

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