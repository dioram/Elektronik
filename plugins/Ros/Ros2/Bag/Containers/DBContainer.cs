using System;
using System.Collections.Generic;
using Elektronik.DataConsumers;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers.SpecialInterfaces;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public abstract class DBContainer<TMessage, TRenderType> : IDBContainer, ISourceTreeNode, IVisible
            where TMessage : RosSharp.RosBridgeClient.Message
    {
        public DBContainer(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                           List<long> actualTimestamps)
        {
            DisplayName = displayName;
            DBModels = dbModels;
            Topic = topic;
            ActualTimestamps = actualTimestamps;
        }

        #region ISourceTreeNode

        public abstract ISourceTreeNode? TakeSnapshot();

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTreeNode> Children { get; } = Array.Empty<ISourceTreeNode>();

        public abstract void Clear();

        public abstract void AddConsumer(IDataConsumer consumer);
        public abstract void RemoveConsumer(IDataConsumer consumer);

        #endregion

        #region IDBContainer

        public long Timestamp { get; private set; } = -1;
        public List<SQLiteConnection> DBModels { get; set; }
        public Topic Topic { get; set; }
        public List<long> ActualTimestamps { get; set; }

        public void ShowAt(long newTimestamp, bool rewind = false)
        {
            if (ActualTimestamps.Count == 0) return;
            var (time, pos) = GetValidTimestamp(newTimestamp);
            if (Timestamp == time) return;
            Timestamp = time;
            _pos = pos;
            if (IsVisible) SetData();
        }

        #endregion

        #region IVisible

        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        public virtual bool ShowButton { get; } = false;
        public event Action<bool>? OnVisibleChanged;

        #endregion

        #region Protected

        protected TRenderType? Current;
        private int _pos;
        private bool _isVisible = true;

        protected (long time, int pos) GetValidTimestamp(long newTimestamp)
        {
            long time = Timestamp;
            int pos = _pos;
            if (newTimestamp > Timestamp)
            {
                for (int i = _pos; i < ActualTimestamps.Count; i++)
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
            var message = this.FindMessage();
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