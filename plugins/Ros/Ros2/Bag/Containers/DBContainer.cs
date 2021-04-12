using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public abstract class DBContainer<TMessage, TRenderType> : IDBContainer, ISourceTree, IVisible
            where TMessage : RosSharp.RosBridgeClient.Message
    {
        public DBContainer(string displayName, SQLiteConnection dbModel, Topic topic, long[] actualTimestamps)
        {
            DisplayName = displayName;
            DBModel = dbModel;
            Topic = topic;
            ActualTimestamps = actualTimestamps;
        }

        #region ISourceTree

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];

        public abstract void Clear();

        public abstract void SetRenderer(object renderer);

        #endregion

        #region IDBContainer

        public long Timestamp { get; private set; } = -1;
        public SQLiteConnection DBModel { get; set; }
        public Topic Topic { get; set; }
        public long[] ActualTimestamps { get; set; }

        public void ShowAt(long newTimestamp, bool rewind = false)
        {
            if (ActualTimestamps.Length == 0) return;
            var (time, pos) = GetValidTimestamp(newTimestamp);
            if (Timestamp == time) return;
            Timestamp = time;
            _pos = pos;
            if (IsVisible) SetData();
        }

        #endregion

        #region IVisible

        public virtual bool IsVisible { get; set; } = true;
        public bool ShowButton { get; } = true;

        #endregion

        #region Protected

        protected TRenderType? Current;
        private int _pos;

        protected (long time, int pos) GetValidTimestamp(long newTimestamp)
        {
            long time = Timestamp;
            int pos = _pos;
            if (newTimestamp > Timestamp)
            {
                for (int i = _pos; i < ActualTimestamps.Length; i++)
                {
                    if (ActualTimestamps[i] > newTimestamp) break;
                    pos = i;
                    time = ActualTimestamps[i];
                }
            }
            else if (newTimestamp < Timestamp)
            {
                for (int i = _pos; i >= 0; i--)
                {
                    pos = i;
                    time = ActualTimestamps[i];
                    if (ActualTimestamps[i] < newTimestamp) break;
                }
            }

            return (time, pos);
        }

        protected virtual void SetData()
        {
            var command = DBModel.CreateCommand("SELECT * FROM messages WHERE topic_id = $id AND timestamp = $time",
                                                Topic.Id, Timestamp);
            var message = command.ExecuteQuery<Message>().First();
            if (message == null)
            {
                Clear();
                return;
            }

            if (MessageParser.Parse(message.Data, Topic.Type, true) is TMessage data)
            {
                Current = ToRenderType(data);
            }
        }

        protected abstract TRenderType ToRenderType(TMessage message);

        #endregion
    }
}