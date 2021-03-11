using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Rosbag2.Data;
using Elektronik.Rosbag2.Parsers;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;

namespace Elektronik.Rosbag2.Containers
{
    public class PointsDBContainer : IContainerTree, IDBContainer
    {
        private bool _isActive = true;

        public PointsDBContainer(string displayName)
        {
            DisplayName = displayName;
        }
        
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
            if (IsActive) SetPoints();
        }
        
        #region IContainerTree implementation

        public void Clear()
        {
            OnClear?.Invoke(this);
        }

        public void SetRenderer(object renderer)
        {
            if (renderer is ICloudRenderer<SlamPoint> pointRenderer)
            {
                OnShow += pointRenderer.ShowItems;
                OnClear += pointRenderer.OnClear;
            }
        }

        public string DisplayName { get; set; }
        public IEnumerable<IContainerTree> Children { get; } = new IContainerTree[0];

        public bool IsActive
        {
            get => _isActive;
            set
            {
                lock (this)
                {
                    if (_isActive == value) return;
                    _isActive = value;
                    if (!_isActive) OnClear?.Invoke(this);
                    else ShowAt(Timestamp);
                }
            }
        }

        #endregion

        #region Private definitinons

        private event Action<object, IEnumerable<SlamPoint>> OnShow;
        private event Action<object> OnClear;
        private int _pos;
        
        private void SetPoints()
        {
            var message = DBModel
                    .Table<Message>()
                    .Where(m => m.Timestamp < Timestamp && m.TopicID == Topic.Id)
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefault();
            if (message == null)
            {
                Clear();
                return;
            }
            var data = MessageParser.Parse(message, Topic) as PointCloud2;
            var points = data.ToSlamPoints();
            if (IsActive) Task.Run(() => OnShow?.Invoke(this, points));
        }

        private (long time, int pos) GetValidTimestamp(long newTimestamp)
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
            else
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

        #endregion
    }
}