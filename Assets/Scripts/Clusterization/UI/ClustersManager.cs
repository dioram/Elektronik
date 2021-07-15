using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Clusterization.Algorithms;
using Elektronik.Clusterization.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
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
                    MainThreadInvoker.Enqueue(() =>
                    {
                        CreateClustersContainers(pair.container as IVisible, pair.name, data);
                        settings.enabled = true;
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
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

        private void CreateClustersContainers(IVisible source, string displayName, List<List<SlamPoint>> data)
        {
            var localizedName = TextLocalizationExtender.GetLocalizedString("Clustered {0}", displayName);
            var clustered = new ClustersContainer(localizedName, data, source);
            
            DataSourcesManager.AddDataSource(clustered);
            foreach (var container in _containers)
            {
                container.IsVisible = false;
            }
            _containers.Add(clustered);
            clustered.OnVisibleChanged += visible => SetVisibility(clustered, source, visible);
            clustered.OnRemoved += () => _containers.Remove(clustered);

            clustered.IsVisible = true;
            source.IsVisible = false;
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