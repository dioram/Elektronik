using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Clouds;
using Elektronik.Clusterization.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using UnityEngine;

namespace Elektronik.Containers
{
    public class CloudContainer<TCloudItem>
            : CloudContainerBase<TCloudItem>, ISourceTree, ILookable, IVisible, ITraceable, IClusterable, ISnapshotable
            where TCloudItem : struct, ICloudItem
    {
        public CloudContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? typeof(TCloudItem).Name : displayName;
        }

        #region IContainer implementation

        public override void Update(TCloudItem item)
        {
            CreateTraces(new[] { item });
            base.Update(item);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public override void Update(IEnumerable<TCloudItem> items)
        {
            CreateTraces(items);
            base.Update(items);
        }

        #endregion

        #region ISourceTree implementations

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public override void SetRenderer(ISourceRenderer renderer)
        {
            _traceContainer.SetRenderer(renderer);
            if (!(renderer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            OnVisibleChanged += visible =>
            {
                if (visible) typedRenderer.OnItemsAdded(this, new AddedEventArgs<TCloudItem>(this));
                else typedRenderer.OnClear(this);
            };
            base.SetRenderer(renderer);
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            lock (Items)
            {
                if (Items.Count == 0) return (transform.position, transform.rotation);

                var min = Vector3.positiveInfinity;
                var max = Vector3.negativeInfinity;
                foreach (var point in Items.Select(i => i.Value.AsPoint().Position))
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
        public int TraceDuration { get; set; } = TraceSettings.Duration;

        #endregion

        #region IClusterable

        public IEnumerable<SlamPoint> GetAllPoints()
        {
            lock (Items)
            {
                return Items.Values.Select(i => i.AsPoint());
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
            lock (Items)
            {
                list = Items.Values.ToList();
            }

            res.AddRange(list);
            return res;
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            recorder.OnAdded(DisplayName, this.ToList());
        }

        #endregion

        #region Private definitions

        private bool _isVisible = true;
        private readonly CloudContainerBase<SimpleLine> _traceContainer = new CloudContainerBase<SimpleLine>();
        private int _maxTraceId = 0;

        private void CreateTraces(IEnumerable<TCloudItem> items)
        {
            if (TraceDuration <= 0 || !TraceEnabled) return;

            var list = items.ToList();
            var traces = new List<SimpleLine>(list.Count());
            lock (_traceContainer)
            {
                foreach (var ci in list)
                {
                    var startPoint = Items[ci.Id].AsPoint();
                    var endPoint = ci.AsPoint();
                    var line = new SimpleLine(_maxTraceId++, startPoint.Position, endPoint.Position, 
                                              startPoint.Color, endPoint.Color);
                    traces.Add(line);
                }
            }

            _traceContainer.AddRange(traces);
            Task.Run(() =>
            {
                Thread.Sleep(TraceDuration);
                lock (_traceContainer)
                {
                    _traceContainer.Remove(traces);
                }
            });
        }

        #endregion
    }
}