using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.DataControllers;
using Elektronik.UI.Windows;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    /// <summary> Controller for all data source plugins. </summary>
    public class DataSourcePluginsController : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private PlayerEventsController PlayerEvents;
        [SerializeField] private GameObject ScreenLocker;
        [SerializeField] private DataSourcesController DataSourcesController;
        [SerializeField] private PluginWindowsManager PluginWindowsManager;
        [SerializeField] private Window ConnectionsWindow;
        [SerializeField] private TMP_Text LoadingErrorLabel;

        #endregion

        /// <summary> Current playing data source plugin. </summary>
        public IDataSourcePlugin CurrentSource { get; private set; }

        /// <summary> Sets given plugin as data source, previous one will be disposed. </summary>
        /// <param name="factory"> Factory from where plugin will be gotten. </param>
        /// <param name="autoPlay"> Should it automatically starts playing. </param>
        public void SetNewSource(IDataSourcePluginsFactory factory, bool autoPlay = true)
        {
            // TODO: If I will use UniTask, this should be the first place to refactor.
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
                    CurrentSource = (IDataSourcePlugin)factory.Start();
                    Plugins.Add(CurrentSource);
                }
                catch (Exception e)
                {
                    ProcessLoadingError(e);
                    return;
                }

                UniRxExtensions.StartOnMainThread(() =>
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
                }).Subscribe().AddTo(this);
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

        #region Private

        private static readonly List<IElektronikPlugin> Plugins = new List<IElektronikPlugin>();

        private void ProcessLoadingError(Exception e)
        {
            Debug.LogException(e);
            UniRxExtensions.StartOnMainThread(() =>
            {
                ScreenLocker.SetActive(false);
                ConnectionsWindow.Show();
                LoadingErrorLabel.text = $"Could not load data source: {e.Message}";
                LoadingErrorLabel.enabled = true;
            }).Subscribe().AddTo(this);
        }

        #endregion
    }
}