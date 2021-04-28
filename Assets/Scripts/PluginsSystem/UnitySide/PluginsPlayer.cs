using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Offline;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsPlayer : MonoBehaviour
    {
        public static ReadOnlyCollection<IElektronikPlugin> Plugins = PluginsLoader.ActivePlugins.AsReadOnly();
        public CSConverter Converter;
        public PlayerEventsManager PlayerEvents;
        public GameObject ScreenLocker;
        public DataSourcesManager DataSourcesManager;

        public event Action PluginsStarted;

        public void ClearMap()
        {
            foreach (var dataSourceOnline in PluginsLoader.ActivePlugins.OfType<IDataSourcePluginOnline>())
            {
                dataSourceOnline.Data.Clear();
            }

            foreach (var dataSourceOffline in PluginsLoader.ActivePlugins.OfType<IDataSourcePluginOffline>())
            {
                dataSourceOffline.StopPlaying();
            }
        }

        public static void MapSourceTree(Action<ISourceTree, string> action)
        {
            foreach (var treeElement in Plugins.OfType<IDataSourcePlugin>().Select(p => p.Data))
            {
                MapSourceTree(treeElement, "", action);
            }
        }

        #region Unity events

        private void Start()
        {
#if UNITY_EDITOR
            if (ModeSelector.Mode == Mode.Online)
            {
                Plugins = PluginsLoader.Plugins.Value
                        .OfType<IDataSourcePluginOnline>()
                        .Select(p => (IElektronikPlugin) p)
                        .ToList()
                        .AsReadOnly();
            }
#endif

            ScreenLocker.SetActive(true);

            foreach (var dataSource in Plugins.OfType<IDataSourcePlugin>())
            {
                dataSource.Converter = Converter;
                DataSourcesManager.AddDataSource(dataSource.Data);
            }

            foreach (var dataSource in Plugins.OfType<IDataSourcePluginOffline>())
            {
                PlayerEvents.SetDataSource(dataSource);
            }

            foreach (var plugin in Plugins)
            {
                var thread = new Thread(() => plugin.Start());
                thread.Start();
                _startupThreads.Add(thread);
            }

            PluginsStarted += () => ScreenLocker.SetActive(false);

            Task.Run(() =>
            {
                foreach (var thread in _startupThreads)
                {
                    thread.Join();
                }

                MainThreadInvoker.Instance.Enqueue(() => PluginsStarted?.Invoke());
            });
        }

        private void Update()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Update(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Stop();
            }
        }

        #endregion

        #region Private

        private readonly List<Thread> _startupThreads = new List<Thread>();

        private static void MapSourceTree(ISourceTree treeElement, string path, Action<ISourceTree, string> action)
        {
            var fullName = $"{path}/{treeElement.DisplayName}";
            action(treeElement, fullName);
            foreach (var child in treeElement.Children)
            {
                MapSourceTree(child, fullName, action);
            }
        }

        #endregion
    }
}