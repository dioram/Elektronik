using System;
using System.Collections.Generic;
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
        public UISettingsGenerator SettingsGenerator;
        public Button CancelButton;
        public Button LoadButton;

        private void Start()
        {
            AttachBehavior2Plugins();
            PluginsListBox.OnSelectionChanged += PluginSelected;

            CancelButton.OnClickAsObservable()
                        .Do(_ => PluginsLoader.ActivePlugins.Clear())
                        .Do(_ => ModeSelector.Mode = Mode.Invalid)
                        .Do(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single))
                        .Subscribe();
            LoadButton.OnClickAsObservable()
                      .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                      .Subscribe();
        }

        private void PluginSelected(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            // TODO: RX style?
            var pluginItem = ((UIPluginListBoxItem) PluginsListBox[e.Index]);
            pluginItem.ToggleDescription();
            SettingsGenerator.Generate(SettingsBag.GetCurrent(pluginItem.Plugin.RequiredSettingsType));
        }

        void AttachBehavior2Plugins()
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
                SettingsBag.CreateCurrent(plugin.RequiredSettingsType);
            }
        }
    }
}