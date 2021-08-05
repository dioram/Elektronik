using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Clusterization.Algorithms;
using Elektronik.Clusterization.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Threading;
using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;

namespace Elektronik.Clusterization.UI
{
    public class ClustersManager : MonoBehaviour
    {
        public TMP_Dropdown ContainersSelector;
        public DataSourcesManager DataSourcesManager;

        public void Compute(MonoBehaviour settings, IClusterizationAlgorithm algorithm)
        {
            var pair = _clusterableContainers[ContainersSelector.value];
            var list = (pair.container as IClusterable)!.GetAllPoints().ToList();

            Task.Run(() =>
            {
                try
                {
                    var data = algorithm.Compute(list);
                    var clustered = CreateClustersContainers(pair.name, data, pair.container as IVisible);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        DataSourcesManager.AddDataSource(clustered);
                        settings.enabled = true;
                    });
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    MainThreadInvoker.Enqueue(() => { settings.enabled = true; });
                }
            });
        }

        #region Unity events

        private void Start()
        {
            DataSourcesManager.MapSourceTree(FindClusterableContainers);
            ContainersSelector.options = _clusterableContainers
                    .Select(c => new TMP_Dropdown.OptionData(c.name))
                    .ToList();
            foreach (var settings in GetComponentsInChildren<AlgorithmSettings>())
            {
                settings.OnComputePressed += Compute;
            }
        }

        #endregion

        #region Private

        private void FindClusterableContainers(ISourceTree element, string fullName)
        {
            if (element is IClusterable) _clusterableContainers.Add((element, fullName));
        }

        private readonly List<(ISourceTree container, string name)> _clusterableContainers
                = new List<(ISourceTree container, string name)>();

        private readonly List<ClustersContainer> _containers = new List<ClustersContainer>();

        private ClustersContainer CreateClustersContainers(string displayName, List<List<SlamPoint>> data,
                                                           IVisible source)
        {
            var clustered = new ClustersContainer($"Clustered {displayName}", data, source);
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