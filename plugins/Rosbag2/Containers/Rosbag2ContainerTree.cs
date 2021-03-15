using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Rosbag2.Data;
using SQLite;

namespace Elektronik.Rosbag2.Containers
{
    public class Rosbag2ContainerTree : VirtualContainer
    {
        public static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/msg/PoseStamped", typeof(TrackedDBContainer)},
            {"nav_msgs/msg/Odometry", typeof(TrackedDBContainer)},
            {"sensor_msgs/msg/PointCloud2", typeof(PointsDBContainer)},
            {"visualization_msgs/msg/MarkerArray", typeof(VisualisationMarkersDBContainer)},
        };

        public Topic[] ActualTopics;

        public Rosbag2ContainerTree(string displayName) : base(displayName)
        {
        }

        public void Init(Rosbag2Settings settings)
        {
            DisplayName = settings.DirPath.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag2: /";
            DBModel = new SQLiteConnection(settings.DirPath, SQLiteOpenFlags.ReadOnly);
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

        public void Reset()
        {
            Clear();
            DBModel.Dispose();
            ChildrenList.Clear();
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

        public void ShowAt(long timestamp, bool rewind = false)
        {
            foreach (var dbContainer in GetRealChildren().OfType<IDBContainer>())
            {
                dbContainer.ShowAt(timestamp, rewind);
            }
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

                var cont = (IContainerTree) Activator.CreateInstance(SupportedMessages[topic.Type], path.Last());
                if (cont is IDBContainer dbContainer)
                {
                    dbContainer.DBModel = DBModel;
                    dbContainer.Topic = topic;
                    var command = DBModel.CreateCommand(
                        "SELECT timestamp FROM messages WHERE topic_id = $id ORDER BY timestamp",
                        topic.Id);
                    var arr = command.ExecuteQueryScalars<long>().ToArray();
                    dbContainer.ActualTimestamps = arr;
                }

                parent.AddChild(cont);
            }
        }

        #endregion
    }
}