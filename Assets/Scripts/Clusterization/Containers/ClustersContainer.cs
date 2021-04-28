﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Localization;
using Object = UnityEngine.Object;

namespace Elektronik.Clusterization.Containers
{
    public class ClustersContainer : ISourceTree, IVisible, IRemovable, IConvexHull
    {
        public ClustersContainer(string displayName, List<List<SlamPoint>> data, IVisible sourceContainer)
        {
            DisplayName = displayName;
            SourceContainer = sourceContainer;
            CreateClusters(data);
            SourceContainer.OnVisibleChanged += visible =>
            {
                if (visible) IsVisible = false;
            };
        }

        public IVisible SourceContainer { get; private set; }

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

        public ISourceTree ContainersOnlyCopy()
        {
            return null;
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

                if (_isVisible) SourceContainer.IsVisible = false;

                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region Removable

        public void RemoveSelf()
        {
            Clear();
            _childrenList.Clear();
            OnRemoved?.Invoke();
            foreach (var hull in Hulls)
            {
                Object.Destroy(hull.gameObject);
            }

            if (_isVisible) SourceContainer.IsVisible = true;
        }

        public event Action OnRemoved;

        #endregion

        #region IConvexHull

        public List<ConvexMesh> Hulls { get; set; } = new List<ConvexMesh>();

        public bool HullVisible
        {
            get => _hullVisible;
            set
            {
                if (_hullVisible == value) return;

                MainThreadInvoker.Instance.Enqueue(() =>
                {
                    for (int i = 0; i < Hulls.Count; i++)
                    {
                        if (_childrenList[i] is IVisible v)
                        {
                            Hulls[i].gameObject.SetActive(value && v.IsVisible);
                        }
                    }
                });
                _hullVisible = value;
                OnHullVisibleChanged?.Invoke(value);
            }
        }

        public event Action<bool> OnHullVisibleChanged;

        #endregion

        #region Private

        private readonly List<object> _renderers = new List<object>();
        private bool _isVisible = true;
        private readonly List<ISourceTree> _childrenList = new List<ISourceTree>();
        private bool _hullVisible = true;

        private void CreateClusters(List<List<SlamPoint>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var cluster = data[i];
                var localizedName = TextLocalizationExtender.GetLocalizedString("Cluster #{0}", i);
                var container = new CloudContainer<SlamPoint>(localizedName);
                foreach (object renderer in _renderers)
                {
                    container.SetRenderer(renderer);
                }

                container.OnVisibleChanged += visible =>
                {
                    var i = _childrenList.IndexOf(container);
                    if (i < 0 || i >= Hulls.Count) return;
                    Hulls[i].gameObject.SetActive(visible && HullVisible);
                };

                container.AddRange(cluster);
                _childrenList.Add(container);
            }
        }

        #endregion
    }
}