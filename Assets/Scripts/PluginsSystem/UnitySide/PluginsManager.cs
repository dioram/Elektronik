using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Offline;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsManager : MonoBehaviour
    {
        public static readonly List<IElektronikPlugin> Plugins = new List<IElektronikPlugin>();
        [SerializeField] private CSConverter Converter;
        [SerializeField] private PlayerEventsManager PlayerEvents;
        [SerializeField] private GameObject ScreenLocker;
        [SerializeField] private DataSourcesManager DataSourcesManager;
        [SerializeField] private PluginWindowsManager PluginWindowsManager;

        public IDataSourcePlugin CurrentSource { get; private set; }

        public void SetNewSource(IDataSourcePluginsFactory factory)
        {
            if (!(CurrentSource is null))
            {
                PluginWindowsManager.UnregisterPlugin(CurrentSource);
                DataSourcesManager.RemoveDataSource(CurrentSource.Data);
                CurrentSource.Dispose();
            }
            IDataSourcePlugin plugin = null;
            Task.Run(() =>
            {
                try
                {
                    plugin = (IDataSourcePlugin) factory.Start(Converter);
                    Plugins.Add(plugin);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                CurrentSource = plugin;
                MainThreadInvoker.Enqueue(() =>
                {
                    PluginWindowsManager.RegisterPlugin(CurrentSource);
                    PlayerEvents.SetDataSource(CurrentSource);
                    DataSourcesManager.AddDataSource(CurrentSource.Data);
                    ScreenLocker.SetActive(false);
                });
            });
        }

        #region Unity events

        private void Start()
        {
            var startupTasks = PluginsLoader.Instance.PluginFactories
                    .Where(f => !(f is IDataSourcePluginsFactory))
                    .Select(f => Task.Run(() => Plugins.Add(f.Start(Converter))))
                    .ToList();
            ScreenLocker.SetActive(true);
            Task.Run(() =>
            {
                foreach (var plugin in Plugins)
                {
                    PluginWindowsManager.RegisterPlugin(plugin);
                }
                Task.WhenAll(startupTasks);
                MainThreadInvoker.Enqueue(() => ScreenLocker.SetActive(false));
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
                plugin.Dispose();
            }
        }

        private void OnApplicationQuit()
        {
#if !UNITY_EDITOR
            System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
        }

        #endregion
    }
}