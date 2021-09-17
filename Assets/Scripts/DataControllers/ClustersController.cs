using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.SpecialInterfaces;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.Threading;
using Elektronik.UI;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.DataControllers
{
    public class ClustersManager : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private TMP_Dropdown ContainersSelector;
        [SerializeField] private DataSourcesController DataSourcesController;
        [SerializeField] private RectTransform AlgorithmPanelsTarget;
        [SerializeField] private GameObject AlgorithmPanelPrefab;
        [SerializeField] private GameObject ToolbarButton;

        #endregion

        #region Unity events

        private void Start()
        {
            var factories = PluginsLoader.Instance.PluginFactories.OfType<IClusterizationAlgorithmFactory>().ToArray();
            if (factories.Length == 0)
            {
                ToolbarButton.SetActive(false);
                enabled = false;
                return;
            }

            UpdateClusterableContainers();
            DataSourcesController.OnSourceAdded += _ => UpdateClusterableContainers();
            DataSourcesController.OnSourceRemoved += _ => UpdateClusterableContainers();

            foreach (var factory in factories)
            {
                var go = Instantiate(AlgorithmPanelPrefab, AlgorithmPanelsTarget);
                var panel = go.GetComponent<ClusterizationAlgorithmPanel>();
                panel.Setup(factory);
                panel.OnComputeRequested
                    .Subscribe(a => Task.Run(() => Compute(a, ChosenContainer, panel)));
            }
        }

        #endregion

        #region Private

        [CanBeNull]
        private IClusterable ChosenContainer
        {
            get
            {
                if (_clusterableContainers.Count == 0
                    || ContainersSelector.value > _clusterableContainers.Count
                    || ContainersSelector.value < 0)
                {
                    return null;
                }

                return _clusterableContainers[ContainersSelector.value].container as IClusterable;
            }
        }

        private async void Compute(IClusterizationAlgorithm algorithm, [CanBeNull] IClusterable container, ClusterizationAlgorithmPanel panel)
        {
            if (container is null) return;
            
            ClustersContainer clustered;
            try
            {
                clustered = await algorithm.ComputeAsync(container);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            SaveClustersContainers(clustered, container as IVisible);

            MainThreadInvoker.Enqueue(() =>
            {
                DataSourcesController.AddDataSource(clustered);
                panel.enabled = true;
            });
        }

        private void UpdateClusterableContainers()
        {
            _clusterableContainers.Clear();
            DataSourcesController.MapSourceTree(FindClusterableContainers);
            ContainersSelector.options = _clusterableContainers
                .Select(c => new TMP_Dropdown.OptionData(c.name))
                .ToList();
        }

        private void FindClusterableContainers(ISourceTreeNode element, string fullName)
        {
            if (element is IClusterable) _clusterableContainers.Add((element, fullName));
        }

        private readonly List<(ISourceTreeNode container, string name)> _clusterableContainers
            = new List<(ISourceTreeNode container, string name)>();

        private readonly List<ClustersContainer> _containers = new List<ClustersContainer>();

        private ClustersContainer SaveClustersContainers(ClustersContainer clustered, IVisible source)
        {
            foreach (var container in _containers)
            {
                container.IsVisible = false;
            }

            _containers.Add(clustered);
            clustered.OnVisibleChanged += visible => SetVisibility(clustered, source, visible);
            clustered.OnRemoved += () => _containers.Remove(clustered);

            clustered.IsVisible = true;
            source.IsVisible = false;
            return clustered;
        }

        private void SetVisibility(ClustersContainer sender, IVisible source, bool isVisible)
        {
            if (!isVisible && !_containers.Any(c => c.IsVisible))
            {
                source.IsVisible = true;
                return;
            }

            if (!isVisible) return;
            foreach (var container in _containers.Where(container => container != sender
                                                                     && container.SourceContainer == source))
            {
                container.IsVisible = false;
            }
        }

        #endregion
    }
}