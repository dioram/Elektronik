using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.Data.Converters;
using Elektronik.DataControllers;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class DataSourcePluginsController : MonoBehaviour
    {
        #region Editor fields
        
        [SerializeField] private CSConverter Converter;
        [SerializeField] private PlayerEventsController PlayerEvents;
        [SerializeField] private GameObject ScreenLocker;
        [SerializeField] private DataSourcesController DataSourcesController;
        [SerializeField] private PluginWindowsManager PluginWindowsManager;

        #endregion

        private static readonly List<IElektronikPlugin> Plugins = new List<IElektronikPlugin>();

        public IDataSourcePlugin CurrentSource { get; private set; }

        public void SetNewSource(IDataSourcePluginsFactory factory, bool autoPlay = true)
        {
            if (!(CurrentSource is null))
            {
                PluginWindowsManager.UnregisterPlugin(CurrentSource);
                DataSourcesController.RemoveDataSource(CurrentSource.Data);
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
                    PlayerEvents.DataSourcePlugin = CurrentSource;
                    DataSourcesController.AddDataSource(CurrentSource.Data);
                    ScreenLocker.SetActive(false);
                    if (autoPlay) CurrentSource.Play();
                });
            });
        }

        #region Unity events

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