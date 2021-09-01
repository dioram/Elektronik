using System.Collections.Generic;
using System.Linq;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public interface IDBContainer
    {
        public long Timestamp { get; }

        public List<SQLiteConnection> DBModels { get; set; }

        public Topic Topic { get; set; }

        public List<long> ActualTimestamps { get; }

        public void ShowAt(long newTimestamp, bool rewind = false);
    }

    // ReSharper disable once InconsistentNaming
    public static class IDBContainerExtensions
    {
        public static Message? FindMessage(this IDBContainer container)
        {
            foreach (var model in container.DBModels)
            {
                var command = model.CreateCommand("SELECT * FROM messages WHERE topic_id = $id AND timestamp = $time",
                                                  container.Topic.Id, container.Timestamp);
                var message = command.ExecuteQuery<Message>().FirstOrDefault();
                if (message is not null) return message;
            }

            return null;
        }

        public static List<Message> FindAllPreviousMessages(this IDBContainer container)
        {
            var query = "select * from messages where topic_id == $id and timestamp <= $timestamp order by timestamp";
            return container.DBModels
                    .Select(model => model.CreateCommand(query, container.Topic.Id, container.Timestamp))
                    .SelectMany(command => command.ExecuteQuery<Message>())
                    .ToList();
        }
    }
}