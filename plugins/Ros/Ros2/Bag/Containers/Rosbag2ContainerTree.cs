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

            var commandTimestamps = DBModel.CreateCommand("SELECT messages.timestamp FROM messages ORDER BY messages.timestamp");
            var commandTopicsIds = DBModel.CreateCommand("SELECT messages.topic_id FROM messages ORDER BY messages.timestamp");
            var timestamps = commandTimestamps.ExecuteQueryScalars<long>().ToArray();
            var topics = commandTopicsIds.ExecuteQueryScalars<int>().ToArray();
            _timestamps = timestamps.Zip(topics, (timestamp, topicId) => (timestamp, topicId)).ToArray();
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

        protected override ISourceTree CreateContainer(string topicName, string topicType)
        {
            var topic = DBModel!.Table<Topic>().First(t => t.Name == topicName);
            var arr = _timestamps
                    .Where(data => data.topicId == topic.Id)
                    .Select(data => data.timestamp)
                    .ToArray();
            return (ISourceTree) Activator.CreateInstance(SupportedMessages[topic.Type],
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
            {"sensor_msgs/msg/Image", typeof(ImageDBContainer)},
            {"std_msgs/msg/String", typeof(StringDBContainer)},
        };

        private (long timestamp, int topicId)[] _timestamps;

        #endregion
    }
}