using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Rosbag2.Data;
using Elektronik.Rosbag2.Parsers;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;
using UnityEngine;

namespace Elektronik.Rosbag2.Containers
{
    public class PointsDBContainer : IContainerTree, IDBContainer, ILookable
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
            _center = new Vector3(float.NaN, float.NaN, float.NaN);
            _bounds = new Vector3(float.NaN, float.NaN, float.NaN);
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
                    if (!_isActive) Clear();
                    else ShowAt(Timestamp);
                }
            }
        }

        #endregion

        #region Private definitinons

        private event Action<object, IEnumerable<SlamPoint>> OnShow;
        private event Action<object> OnClear;
        private int _pos;
        private Vector3 _center = new Vector3(float.NaN, float.NaN, float.NaN);
        private Vector3 _bounds = new Vector3(float.NaN, float.NaN, float.NaN);

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
            CalculateBounds(points);
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

        private void CalculateBounds(SlamPoint[] points)
        {
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;
            foreach (var point in points.Select(p => p.Position))
            {
                min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
            }

            _bounds = max - min;
            _center = (max + min) / 2;
        }

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            if (float.IsNaN(_bounds.x)) return (transform.position, transform.rotation);

            return (_center + _bounds / 2 + _bounds.normalized, Quaternion.LookRotation(-_bounds));
        }
    }
}