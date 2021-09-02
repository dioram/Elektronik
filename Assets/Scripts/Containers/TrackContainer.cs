using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using UnityEngine;

namespace Elektronik.Containers
{
    /// <summary> Contains lines in strict order. </summary>
    public class TrackContainer : IContainer<SimpleLine>, ISourceTree, ILookable, IVisible, ISnapshotable,
                                  IFollowable<SlamTrackedObject>
    {
        public TrackContainer(TrackedObjectsContainer parent, SlamTrackedObject trackedObject)
        {
            _parent = parent;
            _trackedObject = trackedObject;
        }

        public void AddPointToTrack(SlamPoint point) => AddPointToTrack(point.Position, point.Color);

        public void AddPointToTrack(Vector3 pos, Color color)
        {
            SimpleLine line;
            lock (_lines)
            {
                if (_lines.Count == 0)
                {
                    line = new SimpleLine(0, pos, pos, color);
                }
                else
                {
                    var last = _lines[_lines.Count - 1];
                    line = new SimpleLine(_lines.Count, last.EndPos, pos, last.EndColor, color); 
                }
                _lines.Add(line);
            }
            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(line));
        }

        #region IContainer implementaion

        public IEnumerator<SimpleLine> GetEnumerator()
        {
            lock (_lines)
            {
                return _lines.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SimpleLine item)
        {
            lock (_lines)
            {
                _lines.Add(item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(item));
        }

        public void Clear()
        {
            List<int> lines;
            lock (_lines)
            {
                lines = _lines.Select(l => l.Id).ToList();
                _lines.Clear();
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(lines));
        }

        public bool Contains(int id)
        {
            lock (_lines)
            {
                return _lines.Any(l => l.Id == id);
            }
        }


        public bool Contains(SimpleLine item)
        {
            lock (_lines)
            {
                return _lines.Contains(item);
            }
        }

        public void CopyTo(SimpleLine[] array, int arrayIndex)
        {
            lock (_lines)
            {
                _lines.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(SimpleLine item)
        {
            bool res;
            lock (_lines)
            {
                res = _lines.Remove(item);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(item.Id));


            return res;
        }

        public int Count
        {
            get
            {
                lock (_lines)
                {
                    return _lines.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public int IndexOf(SimpleLine item)
        {
            lock (_lines)
            {
                return _lines.IndexOf(item);
            }
        }

        public void Insert(int index, SimpleLine item)
        {
            lock (_lines)
            {
                _lines.Insert(index, item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(item));
        }

        public void RemoveAt(int index)
        {
            lock (_lines)
            {
                _lines.RemoveAt(index);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(index));
        }

        public SimpleLine this[int index]
        {
            get
            {
                lock (_lines)
                {
                    return _lines[index];
                }
            }
            set
            {
                lock (_lines)
                {
                    _lines[index] = value;
                }

                OnUpdated?.Invoke(this, new UpdatedEventArgs<SimpleLine>(value));
            }
        }

        public event EventHandler<AddedEventArgs<SimpleLine>> OnAdded;

        public event EventHandler<UpdatedEventArgs<SimpleLine>> OnUpdated;

        public event EventHandler<RemovedEventArgs> OnRemoved;

        public void AddRange(IList<SimpleLine> items)
        {
            if (items is null) return;
            lock (_lines)
            {
                _lines.AddRange(items);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(items));
        }

        public void Remove(IList<SimpleLine> items)
        {
            if (items is null) return;
            lock (_lines)
            {
                foreach (var item in items)
                {
                    _lines.Remove(item);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(items.Select(i => i.Id).ToList()));
        }

        public IList<SimpleLine> Remove(IList<int> items)
        {
            if (items is null) return new List<SimpleLine>();
            var removed = new List<SimpleLine>();
            lock (_lines)
            {
                foreach (var id in items)
                {
                    var index = _lines.FindIndex(l => l.Id == id);
                    if (index == -1) continue;
                    removed.Add(_lines[index]);
                    _lines.RemoveAt(index);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(items));
            return removed;
        }

        public void Update(SimpleLine item)
        {
            lock (_lines)
            {
                var index = _lines.FindIndex(l => l.Id == item.Id);
                if (index < 0) return;
                _lines[index] = item;
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SimpleLine>(item));
        }

        public void Update(IList<SimpleLine> items)
        {
            if (items is null) return;
            var updated = new List<SimpleLine> { Capacity = items.Count };
            lock (_lines)
            {
                foreach (var item in items)
                {
                    var index = _lines.FindIndex(l => l.Id == item.Id);
                    if (index < 0) continue;
                    updated.Add(item);
                    _lines[index] = item;
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SimpleLine>(updated));
        }

        #endregion

        #region ISourceTree implementation

        public string DisplayName { get; set; } = "Track";

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public void SetRenderer(ISourceRenderer renderer)
        {
            if (renderer is ICloudRenderer<SimpleLine> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                OnVisibleChanged += visible =>
                {
                    if (visible) typedRenderer.OnItemsAdded(this, new AddedEventArgs<SimpleLine>(this));
                    else typedRenderer.OnClear(this);
                };
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(this));
                }
            }
            else if (renderer is TrackedObjectCloud trackedRenderer)
            {
                OnFollowed += trackedRenderer.FollowCamera;
                OnUnfollowed += trackedRenderer.StopFollowCamera;
            }
        }

        #endregion

        #region ILookable

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            Vector3 pos;
            lock (_lines)
            {
                if (_lines.Count == 0) return (transform.position, transform.rotation);

                pos = _lines.Last().EndPos;
            }

            return (pos + (transform.position - pos).normalized, Quaternion.LookRotation(pos - transform.position));
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var res = new TrackContainer(_parent, _trackedObject);
            lock (_lines)
            {
                res.AddRange(_lines);
            }

            return res;
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            lock (_lines)
            {
                recorder.OnAdded(DisplayName, _lines);
            }
        }

        #endregion

        #region IFollowable

        public void Follow()
        {
            IsFollowed = true;
            OnFollowed?.Invoke(this, _parent, _trackedObject);
        }

        public void Unfollow()
        {
            IsFollowed = false;
            OnUnfollowed?.Invoke(_parent, _trackedObject);
        }

        public event Action<IFollowable<SlamTrackedObject>, IContainer<SlamTrackedObject>, SlamTrackedObject>
                OnFollowed;

        public event Action<IContainer<SlamTrackedObject>, SlamTrackedObject> OnUnfollowed;
        public bool IsFollowed { get; private set; }

        #endregion

        #region Private definition

        private readonly List<SimpleLine> _lines = new List<SimpleLine>();
        private bool _isVisible = true;
        private readonly TrackedObjectsContainer _parent;
        private readonly SlamTrackedObject _trackedObject;

        #endregion
    }
}