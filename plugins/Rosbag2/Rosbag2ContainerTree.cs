using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Rosbag2.Data;
using SQLite;

namespace Elektronik.Rosbag2
{
    public class Rosbag2ContainerTree : VirtualContainer
    {
        public static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/msg/PoseStamped", typeof(TrackedObjectsContainer)},
            {"nav_msgs/msg/Odometry", typeof(TrackedObjectsContainer)},
            // {"sensor_msgs/msg/PointCloud2", typeof(CloudContainer<SlamPoint>)},
        };

        public Topic[] ActualTopics;

        public Rosbag2ContainerTree(string displayName) : base(displayName)
        {
        }

        public void Init(string path)
        {
            DisplayName = path.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag2: /";
            DBModel = new SQLiteConnection(path);
            ActualTopics = DBModel.Table<Topic>().ToList().Where(t => SupportedMessages.ContainsKey(t.Type)).ToArray();
            BuildTree();
            Squeeze();

            foreach (var child in Children)
            {
                foreach (var renderer in _renderers)
                {
                    child.SetRenderer(renderer);
                }
            }
        }

        public override string GetFullPath(IContainerTree container)
        {
            var path = base.GetFullPath(container);
            return path.Substring(DisplayName.Length);
        }

        public override void SetRenderer(object renderer)
        {
            _renderers.Add(renderer);
            base.SetRenderer(renderer);
        }

        public SQLiteConnection DBModel { get; private set; }

        #region Private definitions

        private readonly List<object> _renderers = new List<object>();

        private void BuildTree()
        {
            foreach (var topic in ActualTopics)
            {
                var path = topic.Name.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                VirtualContainer parent = this;
                for (int i = 0; i < path.Length - 1; i++)
                {
                    var container = (VirtualContainer) parent.Children.FirstOrDefault(c => c.DisplayName == path[i]);
                    if (container == null)
                    {
                        var newContainer = new VirtualContainer(path[i]);
                        parent.AddChild(newContainer);
                        parent = newContainer;
                    }
                    else
                    {
                        parent = container;
                    }
                }

                parent.AddChild((IContainerTree) Activator.CreateInstance(SupportedMessages[topic.Type], path.Last()));
            }
        }

        #endregion
    }
}