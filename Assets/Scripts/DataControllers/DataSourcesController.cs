using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI;
using Elektronik.UI.Localization;
using Elektronik.UI.Windows;
using JetBrains.Annotations;
using SimpleFileBrowser;
using UnityEngine;

namespace Elektronik.DataControllers
{
    /// <summary> Controller for all data sources. </summary>
    public class DataSourcesController : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private ConsumersRoot ConsumersRoot;
        [SerializeField] [CanBeNull] private Window DataSourceWindow;

        #endregion

        /// <summary> This event is raising every time when new data source was added. </summary>
        public event Action<IDataSource> OnSourceAdded;

        /// <summary> This event is raising every time when data source was removed. </summary>
        public event Action<IDataSource> OnSourceRemoved;

        /// <summary> Adds new consumer. </summary>
        /// <param name="consumer"> New consumer. </param>
        public void AddConsumer(IDataConsumer consumer)
        {
            _consumers.Add(consumer);
            foreach (var source in _dataSources)
            {
                source.AddConsumer(consumer);
            }
        }

        /// <summary> Removes consumer. </summary>
        /// <param name="consumer"> Removed consumer. </param>
        public void RemoveConsumer(IDataConsumer consumer)
        {
            _consumers.Remove(consumer);
            foreach (var source in _dataSources)
            {
                source.RemoveConsumer(consumer);
            }
        }

        /// <summary> Does given action for all sources in tree using dfs. </summary>
        /// <param name="action">
        /// Action that will be done on every source in tree.
        /// It takes source and its name.
        /// </param>
        public void MapSourceTree(Action<IDataSource, string> action)
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

        /// <summary> Does given action for all sources in tree using dfs. </summary>
        /// <param name="action">
        /// Action that will be done on every source in tree.
        /// It takes source and its name.
        /// Return true if you want to go deeper and false otherwise.
        /// </param>
        public void MapSourceTree(Func<IDataSource, string, bool> action)
        {
            foreach (var treeElement in _dataSources)
            {
                MapSourceTree(treeElement, "", action);
            }
        }

        /// <summary> Adds new data source. </summary>
        /// <param name="dataSource"> New data source. </param>
        public void AddDataSource(IDataSource dataSource)
        {
            _dataSources.Add(dataSource);
            foreach (var consumer in _consumers)
            {
                dataSource.AddConsumer(consumer);
            }

            if (dataSource is IRemovableDataSource r)
            {
                r.OnRemoved += () => { _dataSources.Remove(dataSource); };
            }

            if (DataSourceWindow is { }) DataSourceWindow.Show();
            if (_dataSourcesTreeWidget is { }) _dataSourcesTreeWidget.AddDataSource(dataSource);

            OnSourceAdded?.Invoke(dataSource);
        }

        /// <summary> Removes data source. </summary>
        /// <param name="dataSource"> Removed data sources. </param>
        public void RemoveDataSource(IDataSource dataSource)
        {
            _dataSources.Remove(dataSource);
            if (_dataSourcesTreeWidget is { }) _dataSourcesTreeWidget.RemoveDataSource(dataSource);
            OnSourceRemoved?.Invoke(dataSource);
        }

        /// <summary> Takes snapshot of all data sources. </summary>
        public void TakeSnapshot()
        {
            // ReSharper disable once LocalVariableHidesMember
            var name = "Snapshot".tr(_snapshotsCount++);
            var snapshot = new SnapshotContainer(name, _dataSources
                                                         .Select(s => s.TakeSnapshot())
                                                         .Where(ch => ch is { })
                                                         .ToList());
            AddDataSource(snapshot);
        }

        /// <summary> Loads snapshot from file. </summary>
        public void LoadSnapshot()
        {
            FileBrowser.SetFilters(false, PluginsLoader.PluginFactories
                                           .OfType<ISnapshotReaderPluginsFactory>()
                                           .SelectMany(r => r.SupportedExtensions));
            FileBrowser.ShowLoadDialog(LoadSnapshot, () => { },
                                       false, true, "",
                                       "Load snapshot".tr(), "Load".tr());
        }

        #region Unity events

        private void Awake()
        {
            _consumers = ConsumersRoot.GetConsumers();
            if (DataSourceWindow is { })
            {
                _dataSourcesTreeWidget = DataSourceWindow.GetComponent<DataSourcesTreeWidget>();
            }
        }

        #endregion

        #region Private

        private static int _snapshotsCount = 0;
        private readonly List<IDataSource> _dataSources = new List<IDataSource>();
        private List<IDataConsumer> _consumers;
        [CanBeNull] private DataSourcesTreeWidget _dataSourcesTreeWidget;

        private static void MapSourceTree(IDataSource element, string path,
                                          Func<IDataSource, string, bool> action)
        {
            var fullName = $"{path}/{element.DisplayName}";
            var deeper = action(element, fullName);
            if (!deeper) return;
            foreach (var child in element.Children)
            {
                MapSourceTree(child, fullName, action);
            }
        }

        private void LoadSnapshot(string[] files)
        {
            foreach (var path in files)
            {
                var factory = PluginsLoader.PluginFactories
                        .OfType<ISnapshotReaderPluginsFactory>()
                        .FirstOrDefault(pl => pl.SupportedExtensions.Any(e => path.EndsWith(e)));
                if (factory is null) return;
                factory.SetFileName(path);
                var plugin = (IDataSourcePlugin)factory.Start();
                if (plugin is IRewindableDataSource p)
                {
                    p.Position = p.AmountOfFrames - 1;
                }

                AddDataSource(new SnapshotContainer(Path.GetFileName(path), plugin.Data.Children.ToList()));
            }
        }

        #endregion
    }
}