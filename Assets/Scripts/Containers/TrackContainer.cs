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
    public class TrackContainer : IContainer<SlamLine>, ISourceTree, ILookable, IVisible, ISnapshotable, IFollowable<SlamTrackedObject>
    {
        public TrackContainer(TrackedObjectsContainer parent, SlamTrackedObject trackedObject)
        {
            _parent = parent;
            _trackedObject = trackedObject;
        }
        
        #region IContainer implementaion

        public IEnumerator<SlamLine> GetEnumerator()
        {
            lock (_lines)
            {
                return _lines.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SlamLine item)
        {
            lock (_lines)
            {
                _lines.Add(item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {item}));
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


        public bool Contains(SlamLine item)
        {
            lock (_lines)
            {
                return _lines.Contains(item);
            }
        }

        public void CopyTo(SlamLine[] array, int arrayIndex)
        {
            lock (_lines)
            {
                _lines.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(SlamLine item)
        {
            bool res;
            lock (_lines)
            {
                res = _lines.Remove(item);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {item.Id}));


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

        public int IndexOf(SlamLine item)
        {
            lock (_lines)
            {
                return _lines.IndexOf(item);
            }
        }

        public void Insert(int index, SlamLine item)
        {
            lock (_lines)
            {
                _lines.Insert(index, item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {item}));
        }

        public void RemoveAt(int index)
        {
            lock (_lines)
            {
                _lines.RemoveAt(index);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {index}));
        }

        public SlamLine this[int index]
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

                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {value}));
            }
        }

        public event EventHandler<AddedEventArgs<SlamLine>> OnAdded;

        public event EventHandler<UpdatedEventArgs<SlamLine>> OnUpdated;

        public event EventHandler<RemovedEventArgs> OnRemoved;

        public void AddRange(IEnumerable<SlamLine> items)
        {
            if (items is null) return;
            var list = items.ToList();
            lock (_lines)
            {
                _lines.AddRange(list);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(list));
        }

        public void Remove(IEnumerable<SlamLine> items)
        {
            if (items is null) return;
            var list = items.ToList();
            lock (_lines)
            {
                foreach (var item in list)
                {
                    _lines.Remove(item);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(i => i.Id).ToList()));
        }

        public IEnumerable<SlamLine> Remove(IEnumerable<int> items)
        {
            if (items is null) return new List<SlamLine>();
            var list = items.ToList();
            var removed = new List<SlamLine>();
            lock (_lines)
            {
                foreach (var id in list)
                {
                    var index = _lines.FindIndex(l => l.Id == id);
                    if (index == -1) continue;
                    removed.Add(_lines[index]);
                    _lines.RemoveAt(index);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list));
            return removed;
        }

        public void Update(SlamLine item)
        {
            lock (_lines)
            {
                var index = _lines.FindIndex(l => l.Id == item.Id);
                _lines[index] = item;
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {item}));
        }

        public void Update(IEnumerable<SlamLine> items)
        {
            if (items is null) return;
            var list = items.ToList();
            lock (_lines)
            {
                foreach (var item in list)
                {
                    var index = _lines.FindIndex(l => l.Id == item.Id);
                    _lines[index] = item;
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(list));
        }

        #endregion

        #region ISourceTree implementation

        public string DisplayName { get; set; } = "Track";

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public void SetRenderer(ISourceRenderer renderer)
        {
            if (renderer is ICloudRenderer<SlamLine> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                OnVisibleChanged += visible =>
                {
                    if (visible) typedRenderer.OnItemsAdded(this, new AddedEventArgs<SlamLine>(this));
                    else typedRenderer.OnClear(this);
                };
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
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

                pos = _lines.Last().Point2.Position;
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

        public event Action<IFollowable<SlamTrackedObject>, IContainer<SlamTrackedObject>, SlamTrackedObject> OnFollowed;
        public event Action<IContainer<SlamTrackedObject>, SlamTrackedObject> OnUnfollowed;
        public bool IsFollowed { get; private set; }

        #endregion
        
        #region Private definition

        private readonly List<SlamLine> _lines = new List<SlamLine>();
        private bool _isVisible = true;
        private readonly TrackedObjectsContainer _parent;
        private readonly SlamTrackedObject _trackedObject;

        #endregion
    }
}