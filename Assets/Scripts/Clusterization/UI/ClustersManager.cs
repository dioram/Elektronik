using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Clouds;
using Elektronik.Clusterization.Algorithms;
using Elektronik.Clusterization.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI;
using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;

namespace Elektronik.Clusterization.UI
{
    public class ClustersManager : MonoBehaviour
    {
        public PointCloudRenderer Renderer;
        public RectTransform TreeView;
        public GameObject TreeElementPrefab;
        public TMP_Dropdown ContainersSelector;

        public void Compute(IClusterizationAlgorithm algorithm)
        {
            var pair = _clusterableContainers[ContainersSelector.value];
            var list = (pair.container as IClusterable).GetAllPoints().ToList();

            Task.Run(() =>
            {
                try
                {
                    var data = algorithm.Compute(list);
                    MainThreadInvoker.Instance.Enqueue(() =>
                    {
                        var go = Instantiate(TreeElementPrefab, TreeView);
                        var treeElement = go.GetComponent<SourceTreeElement>();
                        var localizedName = TextLocalizationExtender.GetLocalizedString("Clustered {0}",
                            new List<object> {pair.name});
                        var clustered = new ClustersContainer(localizedName, data, pair.container as IVisible);
                        treeElement.Node = clustered;
                        clustered.SetRenderer(Renderer);
                        (pair.container as IVisible).IsVisible = false;
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            });
        }

        #region Unity events

        private void Start()
        {
            PluginsPlayer.MapSourceTree(FindClusterableContainers);
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

        #endregion
    }
}