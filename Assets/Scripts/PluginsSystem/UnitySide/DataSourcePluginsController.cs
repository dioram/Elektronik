using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.Data.Converters;
using Elektronik.DataControllers;
using Elektronik.Threading;
using Elektronik.UI.Windows;
using TMPro;
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
        [SerializeField] private Window ConnectionsWindow;
        [SerializeField] private TMP_Text LoadingErrorLabel;

        #endregion

        private static readonly List<IElektronikPlugin> Plugins = new List<IElektronikPlugin>();

        public IDataSourcePlugin CurrentSource { get; private set; }

        public void SetNewSource(IDataSourcePluginsFactory factory, bool autoPlay = true)
        {
            ScreenLocker.SetActive(true);
            if (!(CurrentSource is null))
            {
                PluginWindowsManager.UnregisterPlugin(CurrentSource);
                DataSourcesController.RemoveDataSource(CurrentSource.Data);
                CurrentSource.Dispose();
            }
            Task.Run(() =>
            {
                try
                {
                    CurrentSource = (IDataSourcePlugin) factory.Start(Converter);
                    Plugins.Add(CurrentSource);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        ScreenLocker.SetActive(false);
                        ConnectionsWindow.Show();
                        LoadingErrorLabel.text = $"Could not load data source: {e.Message}";
                        LoadingErrorLabel.enabled = true;
                    });
                    return;
                }
                MainThreadInvoker.Enqueue(() =>
                {
                    LoadingErrorLabel.enabled = false;
                    PluginWindowsManager.RegisterPlugin(CurrentSource);
                    DataSourcesController.AddDataSource(CurrentSource.Data);
                    ScreenLocker.SetActive(false);
                    if (CurrentSource is IRewindableDataSource source)
                    {
                        PlayerEvents.DataSourcePlugin = source;
                        PlayerEvents.ActivateUI(true);
                        if (autoPlay) source.Play();
                    }
                    else
                    {
                        PlayerEvents.ActivateUI(false);
                    }
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