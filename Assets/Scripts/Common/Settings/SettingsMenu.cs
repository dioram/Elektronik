using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.UI;
using Elektronik.Common.UI;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.Common.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        public UIListBox PluginsListBox;
        public UIListBox HistoryListBox;
        public UISettingsGenerator SettingsGenerator;
        public Button CancelButton;
        public Button LoadButton;
        public Text ErrorLabel;

        private SettingsHistory<SelectedPlugins> _pluginsHistory;
        private SelectedPlugins _selectedPlugins;
        private bool _initCompleted = false;
        private UIPluginListBoxItem _selectedPlugin;

        private void Start()
        {
            AttachBehavior2Plugins();
            PluginsListBox.OnSelectionChanged += PluginSelected;
            HistoryListBox.OnSelectionChanged += RecentSelected;

            CancelButton.OnClickAsObservable()
                    .Do(_ => PluginsLoader.ActivePlugins.Clear())
                    .Do(_ => ModeSelector.Mode = Mode.Invalid)
                    .Do(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single))
                    .Subscribe();
            LoadButton.OnClickAsObservable()
                    .Where(_ => PluginsListBox.AsEnumerable()
                                   .OfType<UIPluginListBoxItem>()
                                   .All(lbi => lbi.Plugin.Settings.Validate()))
                    .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                    .Do(_ => SaveSettings())
                    .Subscribe();

            LoadButton.OnClickAsObservable()
                    .Where(_ => PluginsListBox.AsEnumerable()
                                   .OfType<UIPluginListBoxItem>()
                                   .Any(lbi => !lbi.Plugin.Settings.Validate()))
                    .Subscribe(_ => ShowError());

            SetupSettings();
            _initCompleted = true;
            PluginSelected(this, new UIListBox.SelectionChangedEventArgs(0));
        }

        private void SetupSettings()
        {
            _pluginsHistory = new SettingsHistory<SelectedPlugins>(1);
            if (_pluginsHistory.Recent.Count == 0) _pluginsHistory.Add(new SelectedPlugins());
            _selectedPlugins = (SelectedPlugins) _pluginsHistory.Recent.First();
            foreach (var pluginName in _selectedPlugins.SelectedPluginsNames)
            {
                foreach (var lbi in PluginsListBox.OfType<UIPluginListBoxItem>())
                {
                    if (pluginName == lbi.Plugin.GetType().FullName)
                    {
                        lbi.Toggle(true);
                    }
                }
            }
        }

        private void SaveSettings()
        {
            foreach (var lbi in PluginsListBox.OfType<UIPluginListBoxItem>())
            {
                lbi.Plugin.Settings.ModificationTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                lbi.Plugin.SettingsHistory.Add(lbi.Plugin.Settings);
                lbi.Plugin.SettingsHistory.Save();
            }
        }

        private void PluginSelected(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            _selectedPlugin = ((UIPluginListBoxItem) PluginsListBox[e.Index]);
            _selectedPlugin.ToggleDescription();
            SettingsGenerator.Generate(_selectedPlugin.Plugin.Settings);

            HistoryListBox.Clear();
            foreach (var settings in _selectedPlugin.Plugin.SettingsHistory.Recent)
            {
                var item = (UIRecentSettingsListBoxItem) HistoryListBox.Add();
                item.Data = settings;
            }
        }

        private void ShowError()
        {
            ErrorLabel.enabled = true;
            var plugins = PluginsListBox.AsEnumerable()
                    .OfType<UIPluginListBoxItem>()
                    .Where(lbi => !lbi.Plugin.Settings.Validate())
                    .Select(lbi => lbi.Plugin.DisplayName);
            ErrorLabel.text = $"Wrong settings for plugins: {string.Join(", ", plugins)}";
        }

        private void RecentSelected(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            _selectedPlugin.Plugin.Settings = ((UIRecentSettingsListBoxItem) HistoryListBox[e.Index]).Data;
            SettingsGenerator.Generate(_selectedPlugin.Plugin.Settings);
        }

        private void AttachBehavior2Plugins()
        {
            var availablePlugins = new List<IElektronikPlugin>();
            switch (ModeSelector.Mode)
            {
            case Mode.Online:
                availablePlugins.AddRange(PluginsLoader.Plugins.OfType<IDataSourceOnline>());
                break;
            case Mode.Offline:
                availablePlugins.AddRange(PluginsLoader.Plugins.OfType<IDataSourceOffline>());
                break;
            default:
                break;
            }

            foreach (var plugin in availablePlugins)
            {
                var pluginUIItem = (UIPluginListBoxItem) PluginsListBox.Add();
                pluginUIItem.Plugin = plugin;
                pluginUIItem.OnValueChangedAsObservable()
                        .Where(state => state && ModeSelector.Mode == Mode.Offline)
                        .Subscribe(_ => DisableOfflinePlugins(plugin));

                pluginUIItem.OnValueChangedAsObservable()
                        .Where(state => state && _initCompleted
                                       && !_selectedPlugins.SelectedPluginsNames.Contains(plugin.DisplayName))
                        .Do(_ => _selectedPlugins.SelectedPluginsNames.Add(plugin.GetType().FullName))
                        .Subscribe(_ => _pluginsHistory.Save());
                pluginUIItem.OnValueChangedAsObservable()
                        .Where(state => !state && _initCompleted)
                        .Do(_ => _selectedPlugins.SelectedPluginsNames.Remove(plugin.GetType().FullName))
                        .Subscribe(_ => _pluginsHistory.Save());
            }
        }

        private void DisableOfflinePlugins(IElektronikPlugin except)
        {
            var plugins = PluginsListBox.OfType<UIPluginListBoxItem>()
                    .Where(lbi => lbi.Plugin != except);

            foreach (var plugin in plugins)
            {
                plugin.Toggle(false);
            }
        }
    }
}