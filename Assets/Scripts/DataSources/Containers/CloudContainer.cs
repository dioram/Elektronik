using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.Containers.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    public class CloudContainer<TCloudItem>
            : CloudContainerBase<TCloudItem>, ILookable, IVisible, ITraceable, IClusterable, ISnapshotable
            where TCloudItem : struct, ICloudItem
    {
        public CloudContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? typeof(TCloudItem).Name : displayName;
        }

        #region IContainer

        public override void Update(TCloudItem item)
        {
            CreateTraces(new[] { item });
            base.Update(item);
        }

        public override void Update(IList<TCloudItem> items)
        {
            CreateTraces(items);
            base.Update(items);
        }

        #endregion

        #region ISourceTree

        public override void AddConsumer(IDataConsumer consumer)
        {
            _traceContainer.AddConsumer(consumer);
            if (!(consumer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            _renderers.Add(typedRenderer);
            base.AddConsumer(consumer);
        }

        public override void RemoveConsumer(IDataConsumer consumer)
        {
            _traceContainer.RemoveConsumer(consumer);
            if (!(consumer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            _renderers.Remove(typedRenderer);
            base.RemoveConsumer(consumer);
        }

        #endregion

        #region ILookable

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

        public SlamPoint[] GetAllPoints()
        {
            lock (Items)
            {
                return Items.Values.Select(i => i.AsPoint()).ToArray();
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
                    foreach (var renderer in _renderers)
                    {
                        renderer.OnItemsAdded(this, new AddedEventArgs<TCloudItem>(this));
                    }
                }
                else
                {
                    TCloudItem[] removed;
                    lock (Items)
                    {
                        removed = Items.Values.ToArray();
                    }
                    foreach (var renderer in _renderers)
                    {
                        renderer.OnItemsRemoved(this, new RemovedEventArgs<TCloudItem>(removed));
                    }
                }
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

        #endregion

        #region Private definitions

        private bool _isVisible = true;
        private readonly CloudContainerBase<SimpleLine> _traceContainer = new CloudContainerBase<SimpleLine>();
        private int _maxTraceId = 0;
        private readonly List<ICloudRenderer<TCloudItem>> _renderers = new List<ICloudRenderer<TCloudItem>>(); 

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

            try
            {
                _traceContainer.AddRange(traces);
            }
            catch (ArgumentException)
            {
                // This exception means that we tried to add 2 lines with same ids.
                // Since traces is not very important we can just ignore it.
            }
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