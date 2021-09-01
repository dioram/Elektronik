using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Data;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class Rosbag2ContainerTree : RosContainerTree
    {
        public readonly Dictionary<int, List<long>> Timestamps = new ();
        
        public Rosbag2ContainerTree(Rosbag2Settings settings) : base("")
        {
            DisplayName = settings.FilePath.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag: /";

            DBModels = GetDBFiles(settings.FilePath);
            _actualTopicsSql = DBModels.SelectMany(model => model.Table<Topic>().ToList())
                    .Distinct()
                    .Where(t => SupportedMessages.ContainsKey(t.Type))
                    .ToList();
            ActualTopics = _actualTopicsSql
                    .Select(t => (t.Name, t.Type))
                    .ToList();

            var query = "SELECT messages.timestamp, messages.topic_id FROM messages ORDER BY messages.timestamp";
            var timestamps = DBModels.Select(model => model.CreateCommand(query))
                    .SelectMany(command => command.ExecuteQuery<QueryRow>())
                    .Where(v => _actualTopicsSql.Exists(t => t.Id == v.TopicId))
                    .Select(v => (v.Timestamp, v.TopicId));
            foreach ((long timestamp, int topicId) in timestamps)
            {
                if (!Timestamps.ContainsKey(topicId)) Timestamps.Add(topicId, new List<long>());
                Timestamps[topicId].Add(timestamp);
            }
            RebuildTree();
        }

        private class QueryRow
        {
            [Column("timestamp")]
            public long Timestamp { get; set; }
            
            [Column("topic_id")]
            public int TopicId { get; set; }
        }
        
        public void ShowAt(long timestamp, bool rewind = false)
        {
            foreach (var dbContainer in RealChildren.Values.OfType<IDBContainer>())
            {
                dbContainer.ShowAt(timestamp, rewind);
            }
        }

        public override void Dispose()
        {
            if (DBModels is not null)
            {
                foreach (var model in DBModels)
                {
                    model.Dispose();
                }   
            }
            base.Dispose();
        }

        public List<SQLiteConnection>? DBModels { get; private set; }

        #region Protected

        protected override ISourceTree CreateContainer(string topicName, string topicType)
        {
            var topic = _actualTopicsSql!.First(t => t.Name == topicName);

            List<long> timestamps = new ();
            if (Timestamps.ContainsKey(topic.Id))
            {
                timestamps = Timestamps[topic.Id];
            }
            
            if (SupportedMessages.ContainsKey(topicType))
            {
                return (ISourceTree)Activator.CreateInstance(SupportedMessages[topic.Type],
                                                             topicName.Split('/').Last(),
                                                             DBModels,
                                                             topic,
                                                             timestamps);
            }
            
            return (ISourceTree)Activator.CreateInstance(SupportedMessages["*"],
                                                         topicName.Split('/').Last(),
                                                         DBModels,
                                                         topic,
                                                         timestamps);
        }

        #endregion

        #region Private

        private List<Topic>? _actualTopicsSql;

        private static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            { "geometry_msgs/msg/PoseStamped", typeof(TrackedDBContainer) },
            { "nav_msgs/msg/Odometry", typeof(TrackedDBContainer) },
            { "sensor_msgs/msg/PointCloud2", typeof(PointsDBContainer) },
            { "visualization_msgs/msg/MarkerArray", typeof(VisualisationMarkersDBContainer) },
            { "sensor_msgs/msg/Image", typeof(ImageDBContainer) },
            { "std_msgs/msg/String", typeof(StringDBContainer) },
            { "*", typeof(UnknownTypeDBContainer) }
        };

        private List<SQLiteConnection> GetDBFiles(string path) =>
                Path.GetExtension(path).ToLower() switch
                {
                    ".yaml" => Rosbag2Metadata.ReadFromFile(path)
                            .DBPaths(Path.GetDirectoryName(path)!)
                            .Select(p => new SQLiteConnection(p, SQLiteOpenFlags.ReadOnly))
                            .ToList(),
                    ".db3" => new List<SQLiteConnection> { new(path, SQLiteOpenFlags.ReadOnly) },
                    _ => throw new NotSupportedException("Rosbag2 plugin can open only .yaml and .db3 files")
                };

        #endregion
    }
}