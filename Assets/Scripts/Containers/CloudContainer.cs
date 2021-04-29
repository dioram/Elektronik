using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Clouds;
using Elektronik.Clusterization.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Elektronik.Containers
{
    public class CloudContainer<TCloudItem>
            : IContainer<TCloudItem>, ISourceTree, ILookable, IVisible, ITraceable, IClusterable, ISnapshotable
            where TCloudItem : struct, ICloudItem
    {
        public CloudContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? typeof(TCloudItem).Name : displayName;
        }

        #region IContainer implementation

        public IEnumerator<TCloudItem> GetEnumerator()
        {
            lock (_items)
            {
                return _items.Values.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TCloudItem item)
        {
            lock (_items)
            {
                _items.Add(item.Id, item);
                if (IsVisible)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(new[] {item}));
                }
            }
        }

        public void Clear()
        {
            lock (_items)
            {
                _traceContainer.Clear();
                var ids = _items.Keys.ToList();
                _items.Clear();
                if (IsVisible)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
                }
            }
        }

        public bool Contains(TCloudItem item)
        {
            lock (_items)
            {
                return _items.ContainsKey(item.Id);
            }
        }

        public void CopyTo(TCloudItem[] array, int arrayIndex)
        {
            lock (_items)
            {
                _items.Values.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(TCloudItem item)
        {
            lock (_items)
            {
                RemoveTraces(new[] {item});
                var res = _items.Remove(item.Id);
                if (IsVisible)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {item.Id}));
                }

                return res;
            }
        }

        public int Count
        {
            get
            {
                lock (_items)
                {
                    return _items.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public int IndexOf(TCloudItem item) => item.Id;

        public void Insert(int index, TCloudItem item) => Add(item);

        public void RemoveAt(int index)
        {
            lock (_items)
            {
                RemoveTraces(new[] {_items[index]});
                _items.Remove(index);
                if (IsVisible)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {index}));
                }
            }
        }

        public TCloudItem this[int index]
        {
            get
            {
                lock (_items)
                {
                    return _items[index];
                }
            }
            set
            {
                bool contains;
                lock (_items)
                {
                    contains = _items.ContainsKey(index);
                }

                if (contains)
                {
                    Update(value);
                }
                else
                {
                    Add(value);
                }
            }
        }

        public event Action<IContainer<TCloudItem>, AddedEventArgs<TCloudItem>> OnAdded;
        public event Action<IContainer<TCloudItem>, UpdatedEventArgs<TCloudItem>> OnUpdated;
        public event Action<IContainer<TCloudItem>, RemovedEventArgs> OnRemoved;

        public void AddRange(IEnumerable<TCloudItem> items)
        {
            lock (_items)
            {
                var list = items.ToList();
                foreach (var ci in list)
                {
                    _items.Add(ci.Id, ci);
                }

                if (IsVisible)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(list));
                }
            }
        }

        public void Remove(IEnumerable<TCloudItem> items)
        {
            lock (_items)
            {
                var list = items.ToList();
                RemoveTraces(list);
                foreach (var ci in list)
                {
                    _items.Remove(ci.Id);
                }

                if (IsVisible)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(p => p.Id).ToList()));
                }
            }
        }

        public void Update(TCloudItem item)
        {
            lock (_items)
            {
                CreateTraces(new[] {item});
                _items[item.Id] = item;
                if (IsVisible)
                {
                    OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(new[] {item}));
                }
            }
        }

        public void Update(IEnumerable<TCloudItem> items)
        {
            lock (_items)
            {
                var list = items.ToList();
                CreateTraces(list);
                foreach (var ci in list)
                {
                    _items[ci.Id] = ci;
                }

                if (IsVisible)
                {
                    OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(list));
                }
            }
        }

        #endregion

        #region ISourceTree implementations

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public void SetRenderer(object renderer)
        {
            _traceContainer.SetRenderer(renderer);
            if (renderer is ICloudRenderer<TCloudItem> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(this));
                }
            }
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            lock (_items)
            {
                if (_items.Count == 0) return (transform.position, transform.rotation);

                Vector3 min = Vector3.positiveInfinity;
                Vector3 max = Vector3.negativeInfinity;
                foreach (var point in _items.Select(i => i.Value.AsPoint().Position))
                {
                    min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                    max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
                }

                var bounds = max - min;
                var center = (max + min) / 2;

                return (center + bounds / 2 + bounds.normalized, Quaternion.LookRotation(-bounds));
            }
        }

        #endregion

        #region ITraceable

        public bool TraceEnabled { get; set; }
        public int Duration { get; set; } = TraceSettings.Duration;

        #endregion

        #region IClusterable

        public IEnumerable<SlamPoint> GetAllPoints()
        {
            lock (_items)
            {
                return _items.Values.Select(i => i.AsPoint());
            }
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

                if (_isVisible)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(this));
                    return;
                }

                List<int> items;
                lock (_items)
                {
                    items = _items.Keys.ToList();
                }

                OnRemoved?.Invoke(this, new RemovedEventArgs(items));
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var res = new CloudContainer<TCloudItem>(DisplayName);
            List<TCloudItem> list;
            lock (_items)
            {
                list = _items.Values.ToList();
            }

            res.AddRange(list);
            return res;
        }

        public string Serialize()
        {
            var type = typeof(TCloudItem).Name;
            var converter = new UnityJsonConverter();
            lock (_items)
            {
                return $"{{\"displayName\":\"{DisplayName}\",\"type\":\"{type}\"," +
                        $"\"data\":{JsonConvert.SerializeObject(_items.Values.ToList(), converter)}}}";
            }
        }

        public static CloudContainer<TCloudItem> Deserialize(JToken token)
        {
            var res = new CloudContainer<TCloudItem>(token["displayName"].ToString());
            res.AddRange(JsonConvert.DeserializeObject<TCloudItem[]>(token["data"].ToString()));
            return res;
        }

        #endregion

        #region Private definitions

        private readonly SortedDictionary<int, TCloudItem> _items = new SortedDictionary<int, TCloudItem>();
        private bool _isVisible = true;
        private readonly SlamLinesContainer _traceContainer = new SlamLinesContainer();
        private int _tasks = 0;

        private void RemoveTraces(IEnumerable<TCloudItem> items)
        {
            lock (_traceContainer)
            {
                _traceContainer.Remove(Enumerable.Range(0, _tasks)
                                               .SelectMany(id => items.Select(i => new SlamLine(i.Id, id))));
            }
        }

        private void CreateTraces(IEnumerable<TCloudItem> items)
        {
            if (Duration <= 0 || !TraceEnabled) return;

            var list = items.ToList();
            var traces = new List<SlamLine>(list.Count());
            lock (_traceContainer)
            {
                foreach (var ci in list)
                {
                    var point = ci.AsPoint();
                    point.Id = _tasks;
                    var line = new SlamLine(_items[ci.Id].AsPoint(), point);
                    traces.Add(line);
                }
            }

            _traceContainer.AddRange(traces);
            Task.Run(() =>
            {
                _tasks++;
                Thread.Sleep(Duration);
                lock (_traceContainer)
                {
                    _traceContainer.Remove(traces);
                }

                _tasks--;
            });
        }

        #endregion
    }
}