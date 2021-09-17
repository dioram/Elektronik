using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.DataConsumers;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI;
using Elektronik.UI.Localization;
using SimpleFileBrowser;
using UnityEngine;

namespace Elektronik.DataControllers
{
    public class DataSourcesController : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private RectTransform SourceTreeView;
        [SerializeField] private GameObject TreeElementPrefab;
        [SerializeField] private GameObject RenderersRoot;
        public CSConverter Converter;

        #endregion

        public event Action<ISourceTreeNode> OnSourceAdded;
        public event Action<ISourceTreeNode> OnSourceRemoved;

        // ReSharper disable once ParameterHidesMember
        public void AddRenderer(IDataConsumer consumer)
        {
            _consumers.Add(consumer);
            foreach (var source in _dataSources)
            {
                source.AddConsumer(consumer);
            }
        }

        // ReSharper disable once ParameterHidesMember
        public void RemoveRenderer(IDataConsumer consumer)
        {
            _consumers.Remove(consumer);
            foreach (var source in _dataSources)
            {
                source.RemoveConsumer(consumer);
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
            foreach (var renderer in _consumers)
            {
                source.AddConsumer(renderer);
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
            OnSourceRemoved?.Invoke(source);
        }

        public void TakeSnapshot()
        {
            // ReSharper disable once LocalVariableHidesMember
            var name = "Snapshot".tr(_snapshotsCount++);
            var snapshot = new SnapshotContainer(name, _dataSources
                                                     .Select(s => s.TakeSnapshot())
                                                     .Where(ch => ch is {})
                                                     .ToList());
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

        #region Unity events

        private void Awake()
        {
            _consumers = RenderersRoot.GetComponentsInChildren<IDataConsumer>().ToList();
        }

        #endregion

        #region Private
        
        private static int _snapshotsCount = 0;
        private readonly List<ISourceTreeNode> _dataSources = new List<ISourceTreeNode>();
        private readonly List<SourceTreeElement> _roots = new List<SourceTreeElement>();
        private List<IDataConsumer> _consumers;

        private static void MapSourceTree(ISourceTreeNode treeNodeElement, string path,
                                          Func<ISourceTreeNode, string, bool> action)
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
                var factory = PluginsLoader.Instance.PluginFactories
                    .OfType<ISnapshotReaderPluginsFactory>()
                    .FirstOrDefault(p => p.SupportedExtensions.Any(e => path.EndsWith(e)));
                if (factory is null) return;
                factory.SetFileName(path);
                var plugin = (IDataSourcePlugin)factory.Start(Converter);
                plugin.Position = plugin.AmountOfFrames - 1;
                AddDataSource(new SnapshotContainer(Path.GetFileName(path), plugin.Data.Children.ToList()));
            }
        }

        #endregion
    }
}