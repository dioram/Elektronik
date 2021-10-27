using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources.SpecialInterfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Container for clusters. </summary>
    public class ClustersContainer : IVisibleDataSource, IRemovableDataSource
    {
        /// <summary> Constructor. </summary>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        /// <param name="data"> List of point clusters. </param>
        /// <param name="sourceContainer"> Source of points for clusterization. </param>
        public ClustersContainer(string displayName, IList<IList<SlamPoint>> data, IDataSource sourceContainer)
        {
            DisplayName = displayName;
            ColorizeData(data);
            CreateClusters(data);
            
            if (!(sourceContainer is IVisibleDataSource visibleSource)) return;
            SourceContainer = visibleSource;
            SourceContainer.OnVisibleChanged += visible =>
            {
                if (visible) IsVisible = false;
            };
        }

        /// <summary> Source of points for clusterization. </summary>
        [CanBeNull] public IVisibleDataSource SourceContainer { get; }

        #region IDataSource

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children => _childrenList;

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        /// <inheritdoc />
        public void AddConsumer(IDataConsumer consumer)
        {
            _consumers.Add(consumer);
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        /// <inheritdoc />
        public void RemoveConsumer(IDataConsumer consumer)
        {
            _consumers.Remove(consumer);
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
            }
        }

        /// <inheritdoc />
        public IDataSource TakeSnapshot()
        {
            return new VirtualDataSource(DisplayName, Children
                                         .Select(ch => ch.TakeSnapshot())
                                         .Where(ch => ch is {})
                                         .ToList());
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
                foreach (var child in Children.OfType<IVisibleDataSource>())
                {
                    child.IsVisible = value;
                }

                if (_isVisible && SourceContainer is {}) SourceContainer.IsVisible = false;

                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region IRemovableDataSource

        /// <inheritdoc />
        public void RemoveSelf()
        {
            Clear();
            _childrenList.Clear();
            OnRemoved?.Invoke();

            if (_isVisible && SourceContainer is {}) SourceContainer.IsVisible = true;
        }

        /// <inheritdoc />
        public event Action OnRemoved;

        #endregion

        #region Private

        private readonly List<IDataConsumer> _consumers = new List<IDataConsumer>();
        private bool _isVisible = true;
        private readonly List<IDataSource> _childrenList = new List<IDataSource>();

        private void CreateClusters(IList<IList<SlamPoint>> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var cluster = data[i];
                var container = new CloudCluster(cluster.FirstOrDefault().Color, $"Cluster {i}");
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