using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataSources.Containers.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    public class ClustersContainer : ISourceTreeNode, IVisible, IRemovable
    {
        public ClustersContainer(string displayName, IList<IList<SlamPoint>> data, IVisible sourceContainer)
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

        public IEnumerable<ISourceTreeNode> Children => _childrenList;

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            _consumers.Add(consumer);
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            _consumers.Remove(consumer);
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
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

        private readonly List<IDataConsumer> _consumers = new List<IDataConsumer>();
        private bool _isVisible = true;
        private readonly List<ISourceTreeNode> _childrenList = new List<ISourceTreeNode>();

        private void CreateClusters(IList<IList<SlamPoint>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var cluster = data[i];
                var container = new CloudContainer<SlamPoint>($"Cluster {i}");
                foreach (var renderer in _consumers)
                {
                    container.AddConsumer(renderer);
                }

                container.AddRange(cluster);
                _childrenList.Add(container);
            }
        }

        private void ColorizeData(IList<IList<SlamPoint>> data)
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