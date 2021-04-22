using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class VisualisationMarkersDBContainer : ISourceTree, IDBContainer, IVisible
    {
        public VisualisationMarkersDBContainer(string displayName, SQLiteConnection dbModel, Topic topic,
                                               long[] actualTimestamps)
        {
            DisplayName = displayName;
            DBModel = dbModel;
            Topic = topic;
            ActualTimestamps = actualTimestamps;
        }

        #region ISourceTree implementation

        public void Clear()
        {
            lock (_children)
            {
                foreach (var value in _children.Values)
                {
                    value.Clear();
                }

                _children.Clear();
            }
        }

        public void SetRenderer(object renderer)
        {
            _renderers.Add(renderer);
        }

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTree> Children => _children.Values.ToList();

        #endregion

        #region IDBContainer implementation

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
            if (rewind)
            {
                Rewind();
            }
            else
            {
                AutoRemoveSimple();
                SetPoints();
            }
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                lock (this)
                {
                    if (_isVisible == value) return;
                    _isVisible = value;
                    OnVisibleChanged?.Invoke(_isVisible);
                    foreach (var child in _children.Values.OfType<IVisible>())
                    {
                        child.IsVisible = value;
                    }
                }
            }
        }

        public bool ShowButton => true;
        public event Action<bool>? OnVisibleChanged;

        #endregion

        #region Private definitions

        private bool _isVisible = true;
        private int _pos;
        private readonly SortedDictionary<string, ISourceTree> _children = new();
        private readonly List<object> _renderers = new List<object>();
        private readonly Dictionary<long, List<Marker>> _delayedRemoving = new();

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

        private void AutoRemoveSimple()
        {
            var keys = _delayedRemoving.Keys.Where(k => k < Timestamp);
            foreach (var key in keys)
            {
                DeleteMarkers(_delayedRemoving[key]);
                _delayedRemoving.Remove(key);
            }
        }

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

            var data = (MessageParser.Parse(message.Data, Topic.Type, true) as MarkerArray)!;
            if (data.HasDeleteAll)
            {
                Clear();
                return;
            }

            DeleteMarkers(data.Markers.Where(m => m.Action == Marker.MarkerAction.Delete));
            AddMarkers(data.Markers.Where(m => m.Action == Marker.MarkerAction.Add));
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void DeleteMarkers(IEnumerable<Marker> markers)
        {
            foreach (var marker in markers.Where(m => m.IsSimple))
            {
                if (_children.ContainsKey(marker.Ns))
                    (_children[marker.Ns] as IContainer<SlamPoint>)?.Remove(new SlamPoint {Id = marker.Id});
            }

            foreach (var key in markers.Where(m => m.IsList || m.IsLines)
                    .Select(m => $"{m.Ns} {m.Form} {m.Id}")
                    .Where(k => _children.ContainsKey(k)))
            {
                _children[key].Clear();
                _children.Remove(key);
            }
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void AddMarkers(IEnumerable<Marker> markers)
        {
            foreach (var marker in markers.Where(m => m.IsSimple || m.IsList))
            {
                var key = marker.IsSimple ? marker.Ns : $"{marker.Ns} {marker.Form} {marker.Id}";
                if (!_children.ContainsKey(key)) CreateContainer(typeof(CloudContainer<SlamPoint>), key);
                (_children[key] as IContainer<SlamPoint>)?.AddOrUpdate(marker.GetPoints());
                SetToAutoRemove(marker);
            }

            foreach (var marker in markers.Where(m => m.IsLines))
            {
                var key = marker.IsSimple ? marker.Ns : $"{marker.Ns} {marker.Form} {marker.Id}";
                if (!_children.ContainsKey(key)) CreateContainer(typeof(SlamLinesContainer), key);
                (_children[key] as IContainer<SlamLine>)?.AddOrUpdate(marker.GetLines());
                SetToAutoRemove(marker);
            }
        }

        private void SetToAutoRemove(Marker marker)
        {
            var autoRemove = marker.RemoveAt();
            if (autoRemove <= 0) return;
            if (!_delayedRemoving.ContainsKey(autoRemove))
            {
                _delayedRemoving.Add(autoRemove, new List<Marker>());
            }

            _delayedRemoving[autoRemove].Add(marker);
        }

        private void CreateContainer(Type containerType, string key)
        {
            _children[key] = (ISourceTree) Activator.CreateInstance(containerType, key);
            foreach (var renderer in _renderers)
            {
                _children[key].SetRenderer(renderer);
            }
        }

        private class MarkersComparer : IEqualityComparer<Marker>
        {
            public bool Equals(Marker x, Marker y)
            {
                return x.Ns == y.Ns && x.Id == y.Id && x.Lifetime.ToLong() < y.Header.stamp.ToLong();
            }

            public int GetHashCode(Marker obj)
            {
                unchecked
                {
                    return (obj.Ns.GetHashCode() * 397) ^ obj.Id;
                }
            }
        }

        private void Rewind()
        {
            Clear();
            var message = DBModel
                    .Table<Message>()
                    .Where(m => m.Timestamp <= Timestamp && m.TopicID == Topic.Id)
                    .OrderBy(m => m.Timestamp)
                    .ToArray()
                    .Select(m => (MessageParser.Parse(m.Data, Topic.Type, true) as MarkerArray)!)
                    .ToList();

            var lastClear = message.FindLastIndex(ma => ma.Markers.Any(m => m.Action == Marker.MarkerAction.DeleteAll));
            var added = message.Skip(lastClear + 1).SelectMany(m => m.Markers)
                    .Where(m => m.Action == Marker.MarkerAction.Add && m.RemoveAt() > Timestamp);
            var removed = message.Skip(lastClear + 1).SelectMany(m => m.Markers)
                    .Where(m => m.Action == Marker.MarkerAction.Delete);
            var actual = added.Except(removed, new MarkersComparer());

            AddMarkers(actual);
        }

        #endregion
    }
}