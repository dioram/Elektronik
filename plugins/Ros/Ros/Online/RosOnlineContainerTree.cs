using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros.Online.Handlers;
using Elektronik.Settings;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Rosapi;

namespace Elektronik.RosPlugin.Ros.Online
{
    public class RosOnlineContainerTree : RosContainerTree
    {
        public RosSocket? Socket;

        public RosOnlineContainerTree(string displayName) : base(displayName)
        {
        }

        public void UpdateTopics(TopicsResponse message)
        {
            var tmp = message.topics
                    .Zip(message.types, (s, s1) => (s, s1))
                    .Where(m => SupportedMessages.ContainsKey(m.s1))
                    .OrderBy(m => m.s)
                    .ToArray();

            
            if (tmp.Length == ActualTopics?.Length)
            {
                if (tmp.Length == 0) return;
                bool equals = true;
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].s != ActualTopics[i].Name || tmp[i].s1 != ActualTopics[i].Type)
                    {
                        equals = false;
                    }
                }
                if (equals) return; 
            }
            
            ActualTopics = tmp;
            RebuildTree();
        }

        public void Init(AddressPortScaleSettingsBag settings)
        {
            var uri = $"ws://{settings.IPAddress}:{settings.Port}";
            DisplayName = $"ROS: {uri}";
            Socket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));
        }

        public override void Clear()
        {
            base.Clear();
            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
            _handlers.Clear();
        }

        #region Protected

        protected override IContainerTree CreateContainer(string topicName, string topicType)
        {
            var container = (IContainerTree) Activator.CreateInstance(SupportedMessages[topicType].container,
                                                                      topicName.Split('/').Last());
            _handlers.Add((IMessageHandler) Activator.CreateInstance(SupportedMessages[topicType].handler, container,
                                                                     Socket, topicName));
            return container;
        }

        #endregion

        #region Private

        private static readonly Dictionary<string, (Type container, Type handler)> SupportedMessages = new()
        {
            {"geometry_msgs/PoseStamped", (typeof(TrackedObjectsContainer), typeof(PoseStampedHandler))},
            {"nav_msgs/Odometry", (typeof(TrackedObjectsContainer), typeof(OdometryHandler))},
            {"sensor_msgs/PointCloud2", (typeof(CloudContainer<SlamPoint>), typeof(PointCloud2Handler))},
            // {"visualization_msgs/msg/MarkerArray", typeof(CloudContainer<SlamPoint>)},
        };

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<IMessageHandler> _handlers = new();

        #endregion
    }
}