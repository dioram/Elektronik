using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MarkerCloudRenderer : ICloudRenderer<SlamMarker>
    {
        public MarkerCloudRenderer(IEnumerable<IMarkerCloudRenderer> renderers)
        {
            foreach (var renderer in renderers)
            {
                _renderers[renderer.MarkerType] = renderer;
            }
        }

        #region ICloudRenderer

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                lock (_renderers)
                {
                    foreach (var renderer in _renderers.Values)
                    {
                        renderer.Scale = value;
                    }
                }
            }
        }

        public int ItemsCount
        {
            get
            {
                lock (_renderers)
                {
                    return _renderers.Values.Sum(r => r.ItemsCount);
                }
            }
        }

        public void OnItemsAdded(object sender, AddedEventArgs<SlamMarker> e)
        {
            lock (_renderers)
            {
                AddToChildren(sender, e.AddedItems);
            }
        }

        public void OnItemsUpdated(object sender, UpdatedEventArgs<SlamMarker> e)
        {
            lock (_renderers)
            {
                RemoveFromAnyChildren(sender, e.UpdatedItems);
                AddToChildren(sender, e.UpdatedItems);
            }
        }

        public void OnItemsRemoved(object sender, RemovedEventArgs<SlamMarker> e)
        {
            lock (_renderers)
            {
                RemoveFromChildren(sender, e.RemovedItems);
            }
        }

        public void Dispose()
        {
            foreach (var renderer in _renderers.Values)
            {
                renderer.Dispose();
            }
        }

        #endregion

        #region Private

        private readonly Dictionary<SlamMarker.MarkerType, IMarkerCloudRenderer> _renderers =
                new Dictionary<SlamMarker.MarkerType, IMarkerCloudRenderer>();

        private float _scale;

        private void AddToChildren(object sender, IList<SlamMarker> markers)
        {
            foreach (var group in markers.GroupBy(m => m.Type))
            {
                if (!_renderers.ContainsKey(group.Key)) continue;
                _renderers[group.Key].OnItemsAdded(sender, new AddedEventArgs<SlamMarker>(group.ToArray()));
            }
        }

        private void RemoveFromAnyChildren(object sender, IList<SlamMarker> markers)
        {
            foreach (var renderer in _renderers.Values)
            {
                renderer.OnItemsRemoved(sender, new RemovedEventArgs<SlamMarker>(markers));
            }
        }

        private void RemoveFromChildren(object sender, IList<SlamMarker> markers)
        {
            foreach (var group in markers.GroupBy(m => m.Type))
            {
                if (!_renderers.ContainsKey(group.Key)) continue;
                _renderers[group.Key].OnItemsRemoved(sender, new RemovedEventArgs<SlamMarker>(group.ToArray()));
            }
        }

        #endregion
    }
}