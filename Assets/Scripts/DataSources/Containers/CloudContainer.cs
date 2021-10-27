using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Implementation of cloud container with some popular interfaces. </summary>
    /// <typeparam name="TCloudItem"> Type of cloud items. </typeparam>
    public class CloudContainer<TCloudItem>
            : CloudContainerBase<TCloudItem>, ILookableDataSource, IVisibleDataSource, ITraceableDataSource,
              IClusterableDataSource
            where TCloudItem : struct, ICloudItem
    {
        /// <summary> Constructor. </summary>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        public CloudContainer(string displayName = "") : base(displayName)
        {
        }
        
        #region IContainer

        /// <inheritdoc />
        public override void Update(TCloudItem item)
        {
            CreateTraces(new[] { item });
            base.Update(item);
        }

        /// <inheritdoc />
        public override void Update(IList<TCloudItem> items)
        {
            CreateTraces(items);
            base.Update(items);
        }

        #endregion

        #region IDataSource

        /// <inheritdoc />
        public override void AddConsumer(IDataConsumer consumer)
        {
            _traceCloudContainer.AddConsumer(consumer);
            if (!(consumer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            _renderers.Add(typedRenderer);
            base.AddConsumer(consumer);
        }

        /// <inheritdoc />
        public override void RemoveConsumer(IDataConsumer consumer)
        {
            _traceCloudContainer.RemoveConsumer(consumer);
            if (!(consumer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            _renderers.Remove(typedRenderer);
            base.RemoveConsumer(consumer);
        }

        #endregion

        #region ILookableDataSource

        /// <inheritdoc />
        public Pose Look(Transform transform)
        {
            lock (Items)
            {
                if (Items.Count == 0) return new Pose(transform.position, transform.rotation);

                var min = Vector3.positiveInfinity;
                var max = Vector3.negativeInfinity;
                foreach (var point in Items.Select(i => i.Value.ToPoint().Position))
                {
                    min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                    max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
                }

                var bounds = max - min;
                var center = (max + min) / 2;

                return new Pose(center + bounds / 2 + bounds.normalized, Quaternion.LookRotation(-bounds));
            }
        }

        #endregion

        #region ITraceableDataSource

        public bool TraceEnabled { get; set; }
        public int TraceDuration { get; set; } = TraceSettings.Duration;

        #endregion

        #region IClusterableDataSource

        /// <inheritdoc />
        public SlamPoint[] GetAllPoints()
        {
            lock (Items)
            {
                return Items.Values.Select(i => i.ToPoint()).ToArray();
            }
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

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private

        private bool _isVisible = true;
        private readonly CloudContainerBase<SimpleLine> _traceCloudContainer = new CloudContainerBase<SimpleLine>();
        private int _maxTraceId = 0;
        private readonly List<ICloudRenderer<TCloudItem>> _renderers = new List<ICloudRenderer<TCloudItem>>();

        private void CreateTraces(IEnumerable<TCloudItem> items)
        {
            if (TraceDuration <= 0 || !TraceEnabled) return;

            var list = items.ToList();
            var traces = new List<SimpleLine>(list.Count());
            lock (_traceCloudContainer)
            {
                foreach (var ci in list)
                {
                    var startPoint = Items[ci.Id].ToPoint();
                    var endPoint = ci.ToPoint();
                    var line = new SimpleLine(_maxTraceId++, startPoint.Position, endPoint.Position,
                                              startPoint.Color, endPoint.Color);
                    traces.Add(line);
                }
            }

            try
            {
                _traceCloudContainer.AddRange(traces);
            }
            catch (ArgumentException)
            {
                // This exception means that we tried to add 2 lines with same ids.
                // Since traces is not very important we can just ignore it.
            }

            Task.Run(() =>
            {
                Thread.Sleep(TraceDuration);
                lock (_traceCloudContainer)
                {
                    _traceCloudContainer.Remove(traces);
                }
            });
        }

        #endregion
    }
}