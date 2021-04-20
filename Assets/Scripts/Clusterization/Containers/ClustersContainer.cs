using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Localization;

namespace Elektronik.Clusterization.Containers
{
    public class ClustersContainer : ISourceTree, IVisible, IRemovable
    {
        public ClustersContainer(string displayName, List<List<SlamPoint>> data, IVisible sourceContainer)
        {
            DisplayName = displayName;
            _sourceContainer = sourceContainer;
            CreateClusters(data);
        }

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => _childrenList;

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public void SetRenderer(object renderer)
        {
            _renderers.Add(renderer);
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
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
                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = value;
                }
            }
        }

        public bool ShowButton => true;

        #endregion

        #region Removable

        public void RemoveSelf()
        {
            Clear();
            _childrenList.Clear();
            OnRemoved?.Invoke();
            _sourceContainer.IsVisible = true;
        }

        public event Action OnRemoved;

        #endregion

        #region Private

        private readonly List<object> _renderers = new List<object>();
        private bool _isVisible = true;
        private readonly List<ISourceTree> _childrenList = new List<ISourceTree>();
        private readonly IVisible _sourceContainer;

        private void CreateClusters(List<List<SlamPoint>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var cluster = data[i];
                var localizedName = TextLocalizationExtender.GetLocalizedString("Cluster #{0}", new List<object> {i});
                var container = new CloudContainer<SlamPoint>(localizedName);
                foreach (object renderer in _renderers)
                {
                    container.SetRenderer(renderer);
                }

                container.AddRange(cluster);
                _childrenList.Add(container);
            }
        }

        #endregion
    }
}