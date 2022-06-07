using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Contains lines in strict order. </summary>
    public class TrackCloudContainer : ICloudContainer<SimpleLine>, ILookableDataSource, IVisibleDataSource,
                                       IFollowableDataSource<SlamTrackedObject>
    {
        public TrackCloudContainer(TrackedCloudObjectsContainer parent, SlamTrackedObject trackedObject)
        {
            _parent = parent;
            _trackedObject = trackedObject;
        }

        /// <summary> Creates and adds new line from end of last line to given point. </summary>
        public void AddPointToTrack(SlamPoint point) => AddPointToTrack(point.Position, point.Color);

        
        /// <summary> Creates and adds new line from end of last line to given point. </summary>
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

        #region ICloudContainer

        /// <inheritdoc />
        public IEnumerator<SimpleLine> GetEnumerator()
        {
            lock (_lines)
            {
                return _lines.GetEnumerator();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public void Add(SimpleLine item)
        {
            lock (_lines)
            {
                _lines.Add(item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(item));
        }

        /// <inheritdoc cref="ICloudContainer{T}.Clear" />
        public void Clear()
        {
            SimpleLine[] lines;
            lock (_lines)
            {
                lines = _lines.ToArray();
                _lines.Clear();
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SimpleLine>(lines));
        }

        /// <inheritdoc />
        public bool Contains(int id)
        {
            lock (_lines)
            {
                return _lines.Any(l => l.Id == id);
            }
        }


        /// <inheritdoc />
        public bool Contains(SimpleLine item)
        {
            lock (_lines)
            {
                return _lines.Contains(item);
            }
        }

        /// <inheritdoc />
        public void CopyTo(SimpleLine[] array, int arrayIndex)
        {
            lock (_lines)
            {
                _lines.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public bool Remove(SimpleLine item)
        {
            bool res;
            lock (_lines)
            {
                res = _lines.Remove(item);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SimpleLine>(item));


            return res;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public int IndexOf(SimpleLine item)
        {
            lock (_lines)
            {
                return _lines.IndexOf(item);
            }
        }

        /// <inheritdoc />
        public void Insert(int index, SimpleLine item)
        {
            lock (_lines)
            {
                _lines.Insert(index, item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(item));
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            SimpleLine line;
            lock (_lines)
            {
                line = _lines[index];
                _lines.RemoveAt(index);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SimpleLine>(line));
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

        /// <inheritdoc />
        public event EventHandler<AddedEventArgs<SimpleLine>> OnAdded;

        /// <inheritdoc />
        public event EventHandler<UpdatedEventArgs<SimpleLine>> OnUpdated;

        /// <inheritdoc />
        public event EventHandler<RemovedEventArgs<SimpleLine>> OnRemoved;

        /// <inheritdoc />
        public void AddRange(IList<SimpleLine> items)
        {
            if (items is null) return;
            lock (_lines)
            {
                _lines.AddRange(items);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(items));
        }

        /// <inheritdoc />
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

            OnRemoved?.Invoke(this, new RemovedEventArgs<SimpleLine>(items));
        }

        /// <inheritdoc />
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

            OnRemoved?.Invoke(this, new RemovedEventArgs<SimpleLine>(removed));
            return removed;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        #region IDataSource

        /// <inheritdoc />
        public string DisplayName { get; set; } = "Track";

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children => Enumerable.Empty<IDataSource>();

        /// <inheritdoc />
        public void AddConsumer(IDataConsumer consumer)
        {
            if (consumer is ICloudRenderer<SimpleLine> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                _renderers.Add(typedRenderer);
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SimpleLine>(this));
                }
            }
            else if (consumer is TrackedObjectCloud trackedRenderer)
            {
                OnFollowed += trackedRenderer.FollowCamera;
                OnUnfollowed += trackedRenderer.StopFollowCamera;
            }
        }

        /// <inheritdoc />
        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (!(consumer is ICloudRenderer<SimpleLine> typedRenderer)) return;
            OnAdded -= typedRenderer.OnItemsAdded;
            OnUpdated -= typedRenderer.OnItemsUpdated;
            OnRemoved -= typedRenderer.OnItemsRemoved;
            _renderers.Remove(typedRenderer);
        }

        /// <inheritdoc />
        public IDataSource TakeSnapshot()
        {
            var res = new TrackCloudContainer(_parent, _trackedObject);
            lock (_lines)
            {
                res.AddRange(_lines);
            }

            return res;
        }

        #endregion

        #region ILookableDataSource

        /// <inheritdoc />
        public Pose Look(Transform transform)
        {
            Vector3 pos;
            lock (_lines)
            {
                if (_lines.Count == 0) return new Pose(transform.position, transform.rotation);

                pos = _lines.Last().EndPos;
            }

            return new Pose(pos + (transform.position - pos).normalized,
                            Quaternion.LookRotation(pos - transform.position));
        }

        #endregion

        #region IVisibleDataSource

        /// <inheritdoc />
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);

                if (_isVisible)
                {
                    foreach (var renderer in _renderers)
                    {
                        renderer.OnItemsAdded(this, new AddedEventArgs<SimpleLine>(this));
                    }

                    return;
                }

                foreach (var renderer in _renderers)
                {
                    lock (_lines)
                    {
                        renderer.OnItemsRemoved(this, new RemovedEventArgs<SimpleLine>(_lines));
                    }
                }
            }
        }

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region IFollowableDataSource

        /// <inheritdoc />
        public void Follow()
        {
            IsFollowed = true;
            OnFollowed?.Invoke(this, _parent, _trackedObject);
        }

        /// <inheritdoc />
        public void Unfollow()
        {
            IsFollowed = false;
            OnUnfollowed?.Invoke(_parent, _trackedObject);
        }

        /// <inheritdoc />
        public event Action<IFollowableDataSource<SlamTrackedObject>, ICloudContainer<SlamTrackedObject>, SlamTrackedObject>
                OnFollowed;

        /// <inheritdoc />
        public event Action<ICloudContainer<SlamTrackedObject>, SlamTrackedObject> OnUnfollowed;

        /// <inheritdoc />
        public bool IsFollowed { get; private set; }

        #endregion

        #region Private

        private readonly List<SimpleLine> _lines = new List<SimpleLine>();
        private bool _isVisible = true;
        private readonly TrackedCloudObjectsContainer _parent;
        private readonly SlamTrackedObject _trackedObject;
        private readonly List<ICloudRenderer<SimpleLine>> _renderers = new List<ICloudRenderer<SimpleLine>>();

        #endregion
    }
}