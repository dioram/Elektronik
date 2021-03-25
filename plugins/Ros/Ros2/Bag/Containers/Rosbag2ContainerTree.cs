using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using Elektronik.Settings;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class Rosbag2ContainerTree : RosContainerTree
    {
        public Rosbag2ContainerTree(string displayName) : base(displayName)
        {
        }

        public void Init(FileScaleSettingsBag settings)
        {
            DisplayName = settings.FilePath.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag: /";
            DBModel = new SQLiteConnection(settings.FilePath, SQLiteOpenFlags.ReadOnly);
            ActualTopics = DBModel.Table<Topic>()
                    .ToList()
                    .Where(t => SupportedMessages.ContainsKey(t.Type))
                    .Select(t => (t.Name, t.Type))
                    .ToList();
            RebuildTree();
        }
        
        public void ShowAt(long timestamp, bool rewind = false)
        {
            foreach (var dbContainer in RealChildren.Values.OfType<IDBContainer>())
            {
                dbContainer.ShowAt(timestamp, rewind);
            }
        }

        public override void Reset()
        {
            DBModel?.Dispose();
            base.Reset();
        }

        public SQLiteConnection? DBModel { get; private set; }

        #region Protected

        protected override IContainerTree CreateContainer(string topicName, string topicType)
        {
            var topic = DBModel!.Table<Topic>().First(t => t.Name == topicName);
            var command = DBModel.CreateCommand(
                "SELECT timestamp FROM messages WHERE topic_id = $id ORDER BY timestamp",
                topic.Id);
            var arr = command.ExecuteQueryScalars<long>().ToArray();
            return (IContainerTree) Activator.CreateInstance(SupportedMessages[topic.Type],
                                                             topicName.Split('/').Last(),
                                                             DBModel,
                                                             topic,
                                                             arr);
        }

        #endregion

        #region Private
        
        private static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/msg/PoseStamped", typeof(TrackedDBContainer)},
            {"nav_msgs/msg/Odometry", typeof(TrackedDBContainer)},
            {"sensor_msgs/msg/PointCloud2", typeof(PointsDBContainer)},
            {"visualization_msgs/msg/MarkerArray", typeof(VisualisationMarkersDBContainer)},
        };

        #endregion
    }
}