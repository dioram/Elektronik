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
            Camera.main.transform.parent = null;
            
            foreach (var dataSourceOffline in Plugins.OfType<IDataSourcePluginOffline>())
            {
                dataSourceOffline.StopPlaying();
            }
            DataSourcesManager.ClearMap();
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

                MainThreadInvoker.Enqueue(() => PluginsStarted?.Invoke());
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

        private void OnApplicationQuit()
        {
#if !UNITY_EDITOR
            System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
        }

        #endregion

        #region Private

        private readonly List<Thread> _startupThreads = new List<Thread>();

        #endregion
    }
}