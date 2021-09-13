using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI;
using Elektronik.UI.Localization;
using SimpleFileBrowser;
using UnityEngine;

namespace Elektronik.Data
{
    public class DataSourcesManager : MonoBehaviour
    {
        public CSConverter Converter;
        private static int _snapshotsCount = 0;
        [SerializeField] private RectTransform SourceTreeView;
        [SerializeField] private GameObject TreeElementPrefab;
        [SerializeField] private GameObject RenderersRoot;

        private readonly List<ISourceTreeNode> _dataSources = new List<ISourceTreeNode>();
        private readonly List<SourceTreeElement> _roots = new List<SourceTreeElement>();
        private List<ISourceRenderer> _renderers;

        public event Action<ISourceTreeNode> OnSourceAdded;

        // ReSharper disable once ParameterHidesMember
        public void AddRenderer(ISourceRenderer renderer)
        {
            _renderers.Add(renderer);
            foreach (var source in _dataSources)
            {
                source.AddRenderer(renderer);
            }
        }

        // ReSharper disable once ParameterHidesMember
        public void RemoveRenderer(ISourceRenderer renderer)
        {
            _renderers.Remove(renderer);
            foreach (var source in _dataSources)
            {
                source.RemoveRenderer(renderer);
            }
        }

        public void ClearMap()
        {
            var removable = _dataSources.OfType<IRemovable>().ToList();
            foreach (var r in removable)
            {
                r.RemoveSelf();
            }

            foreach (var source in _dataSources)
            {
                source.Clear();
            }
        }

        public void MapSourceTree(Action<ISourceTreeNode, string> action)
        {
            foreach (var treeElement in _dataSources)
            {
                MapSourceTree(treeElement, "", (tree, s) =>
                {
                    action(tree, s);
                    return true;
                });
            }
        }

        /// <summary> Maps tree using dfs. </summary>
        /// <param name="action">
        /// Action for each node. Return true if you want to go deeper and false otherwise.
        /// </param>
        public void MapSourceTree(Func<ISourceTreeNode, string, bool> action)
        {
            foreach (var treeElement in _dataSources)
            {
                MapSourceTree(treeElement, "", action);
            }
        }

        private void Awake()
        {
            _renderers = RenderersRoot.GetComponentsInChildren<ISourceRenderer>().ToList();
        }

        public void AddDataSource(ISourceTreeNode source)
        {
            _dataSources.Add(source);
            var treeElement = Instantiate(TreeElementPrefab, SourceTreeView).GetComponent<SourceTreeElement>();
            _roots.Add(treeElement);
            treeElement.Node = source;
            if (_roots.Count == 1)
            {
                treeElement.ChangeState();
            }

            if (source is IRemovable r)
            {
                r.OnRemoved += () =>
                {
                    _dataSources.Remove(source);
                    Destroy(treeElement.gameObject);
                };
            }

            // ReSharper disable once LocalVariableHidesMember
            foreach (var renderer in _renderers)
            {
                source.AddRenderer(renderer);
            }

            OnSourceAdded?.Invoke(source);
        }

        public void RemoveDataSource(ISourceTreeNode source)
        {
            _dataSources.Remove(source);
            var treeElement = _roots.Find(r => r.Node == source);
            if (treeElement is null) return;
            _roots.Remove(treeElement);
            Destroy(treeElement.gameObject);
        }

        public void TakeSnapshot()
        {
            // ReSharper disable once LocalVariableHidesMember
            var name = "Snapshot".tr(_snapshotsCount++);
            var snapshot = new SnapshotContainer(name,
                                                 _dataSources
                                                         .OfType<ISnapshotable>()
                                                         .Select(s => s.TakeSnapshot())
                                                         .Select(s => s as ISourceTreeNode)
                                                         .ToList(),
                                                 Converter);
            AddDataSource(snapshot);
        }

        public void LoadSnapshot()
        {
            FileBrowser.SetFilters(false, PluginsLoader.Instance.PluginFactories
                                           .OfType<ISnapshotReaderPluginsFactory>()
                                           .SelectMany(r => r.SupportedExtensions));
            FileBrowser.ShowLoadDialog(LoadSnapshot,
                                       () => { },
                                       false,
                                       true,
                                       "",
                                       "Load snapshot".tr(),
                                       "Load".tr());
        }

        #region Private

        private static void MapSourceTree(ISourceTreeNode treeNodeElement, string path, Func<ISourceTreeNode, string, bool> action)
        {
            var fullName = $"{path}/{treeNodeElement.DisplayName}";
            var deeper = action(treeNodeElement, fullName);
            if (!deeper) return;
            foreach (var child in treeNodeElement.Children)
            {
                MapSourceTree(child, fullName, action);
            }
        }

        private void LoadSnapshot(string[] files)
        {
            foreach (var path in files)
            {
                var playerPrefab = PluginsLoader.Instance.PluginFactories
                        .OfType<ISnapshotReaderPluginsFactory>()
                        .FirstOrDefault(p => p.SupportedExtensions.Any(e => path.EndsWith(e)));
                if (playerPrefab is null) return;
                var factory = (ISnapshotReaderPluginsFactory) Activator.CreateInstance(playerPrefab.GetType());
                factory.SetFileName(path);
                var plugin = (IDataSourcePlugin) factory.Start(Converter);
                plugin.Position = plugin.AmountOfFrames - 1;
                AddDataSource(new SnapshotContainer(Path.GetFileName(path), plugin.Data.Children, Converter));
            }
        }

        #endregion
    }
}