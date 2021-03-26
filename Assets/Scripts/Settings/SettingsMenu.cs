using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        public ListBox PluginsListBox;
        public ListBox HistoryListBox;
        public SettingsGenerator SettingsGenerator;
        public Button CancelButton;
        public Button LoadButton;
        public Text ErrorLabel;

        private SettingsHistory<SelectedPlugins> _pluginsHistory;
        private SelectedPlugins _selectedPlugins;
        private bool _initCompleted = false;
        private PluginListBoxItem _selectedPlugin;

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
                    .Select(_ => PluginsLoader.ActivePlugins)
                    .Where(ps => ps.Count > 0)
                    .Where(ps => ps.All(p => p.Settings.Validate()))
                    .Do(_ => SaveSettings())
                    .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                    .Subscribe();

            LoadButton.OnClickAsObservable()
                    .Select(_ => PluginsLoader.ActivePlugins)
                    .Where(ps => ps.Count == 0)
                    .Subscribe(_ =>
                    {
                        ErrorLabel.enabled = true;
                        ErrorLabel.text = $"No data source selected";
                    });

            LoadButton.OnClickAsObservable()
                    .Select(_ => PluginsLoader.ActivePlugins)
                    .Where(ps => ps.Any(p => !p.Settings.Validate()))
                    .Subscribe(_ => ShowError());

            SetupSettings();
            _initCompleted = true;

            var selectedIndex = 0;
            for (int i = 0; i < PluginsListBox.Count(); i++)
            {
                var lbi = (PluginListBoxItem) PluginsListBox[i];
                if (lbi.State)
                {
                    selectedIndex = i;
                    break;
                }
            }
            PluginSelected(this, new ListBox.SelectionChangedEventArgs(selectedIndex));
        }

        private void SetupSettings()
        {
            _pluginsHistory = new SettingsHistory<SelectedPlugins>("SelectedPlugins.json", 1);
            if (_pluginsHistory.Recent.Count == 0) _pluginsHistory.Add(new SelectedPlugins());
            _selectedPlugins = (SelectedPlugins) _pluginsHistory.Recent.First();
            foreach (var pluginName in _selectedPlugins.SelectedPluginsNames)
            {
                foreach (var lbi in PluginsListBox.OfType<PluginListBoxItem>())
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
            foreach (var lbi in PluginsListBox.OfType<PluginListBoxItem>())
            {
                lbi.Plugin.Settings.ModificationTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                lbi.Plugin.SettingsHistory.Add(lbi.Plugin.Settings.Clone());
                lbi.Plugin.SettingsHistory.Save();
            }
        }

        private void PluginSelected(object sender, ListBox.SelectionChangedEventArgs e)
        {
            _selectedPlugin = ((PluginListBoxItem) PluginsListBox[e.Index]);
            _selectedPlugin.ToggleDescription();
            SettingsGenerator.Generate(_selectedPlugin.Plugin.Settings);

            HistoryListBox.Clear();
            foreach (var settings in _selectedPlugin.Plugin.SettingsHistory.Recent)
            {
                var item = (RecentSettingsListBoxItem) HistoryListBox.Add();
                item.Data = settings;
            }
        }

        private void ShowError()
        {
            ErrorLabel.enabled = true;
            var plugins = PluginsListBox.AsEnumerable()
                    .OfType<PluginListBoxItem>()
                    .Where(lbi => !lbi.Plugin.Settings.Validate())
                    .Select(lbi => lbi.Plugin.DisplayName);
            ErrorLabel.text = $"Wrong settings for plugins: {string.Join(", ", plugins)}";
        }

        private void RecentSelected(object sender, ListBox.SelectionChangedEventArgs e)
        {
            _selectedPlugin.Plugin.Settings = ((RecentSettingsListBoxItem) HistoryListBox[e.Index]).Data.Clone();
            SettingsGenerator.Generate(_selectedPlugin.Plugin.Settings);
        }

        private void AttachBehavior2Plugins()
        {
            var availablePlugins = new List<IElektronikPlugin>();
            switch (ModeSelector.Mode)
            {
            case Mode.Online:
                availablePlugins.AddRange(PluginsLoader.Plugins.OfType<IDataSourcePluginOnline>());
                break;
            case Mode.Offline:
                availablePlugins.AddRange(PluginsLoader.Plugins.OfType<IDataSourcePluginOffline>());
                break;
            }

            foreach (var plugin in availablePlugins)
            {
                var pluginUIItem = (PluginListBoxItem) PluginsListBox.Add();
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
            var plugins = PluginsListBox.OfType<PluginListBoxItem>()
                    .Where(lbi => lbi.Plugin != except);

            foreach (var plugin in plugins)
            {
                plugin.Toggle(false);
            }
        }
    }
}