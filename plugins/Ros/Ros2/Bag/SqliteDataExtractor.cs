using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.RosPlugin.Bag;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class SqliteDataExtractor : IDataExtractor<long>
    {
        private readonly SQLiteConnection _dbModel;
        private readonly IEnumerable<(long timestamp, int topicId)> _allTimestamps;

        public SqliteDataExtractor(SQLiteConnection dbModel)
        {
            _dbModel = dbModel;
            _allTimestamps = GetTimestamps();
        }

        public IEnumerable<long> GetAllMessagePositionsFor(string topic)
        {
            var topicId = _dbModel.Table<Topic>().First(t => t.Name == topic).Id;
            return _allTimestamps.Where(p => p.topicId == topicId).Select(p => p.timestamp);
        }

        public Task<RosMessage?> GetMessage(long pos, string topicName, string topicType)
        {
            var topicId = _dbModel.Table<Topic>().First(t => t.Name == topicName).Id;
            //TODO: < 
            var command = _dbModel.CreateCommand("SELECT * FROM messages WHERE topic_id = $id AND timestamp = $time",
                                                 topicId, pos);
            var message = command.ExecuteQuery<Message>().First();
            return Task.Run(() => MessageParser.Parse(message.Data, topicType, true));
        }

        #region Private

        private IEnumerable<(long, int)> GetTimestamps()
        {
            var commandTimestamps =
                    _dbModel.CreateCommand("SELECT messages.timestamp FROM messages ORDER BY messages.timestamp");
            var commandTopicsIds =
                    _dbModel.CreateCommand("SELECT messages.topic_id FROM messages ORDER BY messages.timestamp");
            var timestamps = commandTimestamps.ExecuteQueryScalars<long>();
            var topics = commandTopicsIds.ExecuteQueryScalars<int>();
            foreach (var pair in timestamps.Zip(topics, (timestamp, topicId) => (timestamp, topicId)))
            {
                yield return pair;
            }
        }

        #endregion
    }
}