using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elektronik.Clouds;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI;
using Elektronik.UI.Localization;
using Elektronik.UI.Windows;
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
        [SerializeField] private GameObject Renderers;

        private readonly List<ISourceTree> _dataSources = new List<ISourceTree>();
        private readonly List<SourceTreeElement> _roots = new List<SourceTreeElement>();
        private Component[] _renderers;

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

        public ISourceTree GetByPath(string fullName)
        {
            var names = fullName.Split('/');

            var child = FindChildWithName(names[0]);
            foreach (var n in names.Skip(1))
            {
                if (child == null) return null;
                child = FindChildWithName(child, n);
            }

            return child;
        }

        public void MapSourceTree(Action<ISourceTree, string> action)
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
        public void MapSourceTree(Func<ISourceTree, string, bool> action)
        {
            foreach (var treeElement in _dataSources)
            {
                MapSourceTree(treeElement, "", action);
            }
        }

        private void Awake()
        {
            _renderers = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .Any(i => i.GetGenericTypeDefinition() == typeof(ICloudRenderer<>)))
                    .SelectMany(t => Renderers.GetComponentsInChildren(t))
                    .Concat(new[] {Renderers.transform.Find("Windows").GetComponent<WindowsManager>()})
                    .ToArray();
        }

        public void AddDataSource(ISourceTree source)
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
                source.SetRenderer(renderer);
            }
        }

        public void TakeSnapshot()
        {
            // ReSharper disable once LocalVariableHidesMember
            var name = TextLocalizationExtender.GetLocalizedString("Snapshot", _snapshotsCount++);
            var snapshot = new SnapshotContainer(name,
                                                 _dataSources
                                                         .OfType<ISnapshotable>()
                                                         .Select(s => s.TakeSnapshot())
                                                         .Select(s => s as ISourceTree)
                                                         .ToList(),
                                                 Converter);
            AddDataSource(snapshot);
        }

        public void LoadSnapshot()
        {
            FileBrowser.SetFilters(false, PluginsLoader.Plugins.Value
                                           .OfType<IDataSourcePluginOffline>()
                                           .SelectMany(r => r.SupportedExtensions));
            FileBrowser.ShowLoadDialog(LoadSnapshot,
                                       () => { },
                                       false,
                                       true,
                                       "",
                                       TextLocalizationExtender.GetLocalizedString("Load snapshot"),
                                       TextLocalizationExtender.GetLocalizedString("Load"));
        }

        #region Private

        private ISourceTree FindChildWithName(ISourceTree container, string name)
        {
            return container.Children.FirstOrDefault(ch => ch.DisplayName == name);
        }

        private ISourceTree FindChildWithName(string name)
        {
            return _dataSources.FirstOrDefault(ch => ch.DisplayName == name);
        }

        private static void MapSourceTree(ISourceTree treeElement, string path, Func<ISourceTree, string, bool> action)
        {
            var fullName = $"{path}/{treeElement.DisplayName}";
            bool deeper = action(treeElement, fullName);
            if (!deeper) return;
            foreach (var child in treeElement.Children)
            {
                MapSourceTree(child, fullName, action);
            }
        }

        private void LoadSnapshot(string[] files)
        {
            foreach (var path in files)
            {
                var playerPrefab = PluginsLoader.Plugins.Value
                        .OfType<IDataSourcePluginOffline>()
                        .FirstOrDefault(p => p.SupportedExtensions.Any(e => path.EndsWith(e)));
                if (playerPrefab is null) return;
                var player = (IDataSourcePluginOffline) Activator.CreateInstance(playerPrefab.GetType());
                player.Converter = Converter;
                player.SetFileName(path);
                player.Start();
                player.CurrentPosition = player.AmountOfFrames - 1;
                AddDataSource(new SnapshotContainer(Path.GetFileName(path), player.Data.Children, Converter));
            }
        }

        #endregion
    }
}