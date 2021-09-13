using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clusterization.Containers
{
    public class ClustersContainer : ISourceTree, IVisible, IRemovable
    {
        public ClustersContainer(string displayName, List<List<SlamPoint>> data, IVisible sourceContainer)
        {
            DisplayName = displayName;
            SourceContainer = sourceContainer;
            ColorizeData(data);
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

        public void SetRenderer(ISourceRenderer renderer)
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

                if (_isVisible) SourceContainer.IsVisible = false;

                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region IRemovable

        public void RemoveSelf()
        {
            Clear();
            _childrenList.Clear();
            OnRemoved?.Invoke();

            if (_isVisible) SourceContainer.IsVisible = true;
        }

        public event Action OnRemoved;

        #endregion

        #region Private

        private readonly List<ISourceRenderer> _renderers = new List<ISourceRenderer>();
        private bool _isVisible = true;
        private readonly List<ISourceTree> _childrenList = new List<ISourceTree>();

        private void CreateClusters(List<List<SlamPoint>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var cluster = data[i];
                var container = new CloudContainer<SlamPoint>($"Cluster {i}");
                foreach (ISourceRenderer renderer in _renderers)
                {
                    container.SetRenderer(renderer);
                }

                container.AddRange(cluster);
                _childrenList.Add(container);
            }
        }

        private void ColorizeData(List<List<SlamPoint>> data)
        {
            var colors = GenerateColors(data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    data[i][j] = new SlamPoint(data[i][j].Id, data[i][j].Position, colors[i], data[i][j].Message);
                }
            }
        }

        private List<Color> GenerateColors(int num)
        {
            var rand = new System.Random();
            var colors = new List<Color>
            {
                Color.blue,
                Color.red,
                Color.green,
                Color.yellow,
                Color.magenta,
                new Color(0.5f, 0.5f, 1f),
                new Color(1f, 0.5f, 0),
                new Color(0.5f, 0, 1)
            };
            for (int i = colors.Count; i < num; i++)
            {
                colors.Add(new Color((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble()));
            }

            return colors;
        }

        #endregion
    }
}