using System.Collections.Generic;
using Elektronik.UI.Buttons;
using Elektronik.UI.SettingsFields;
using Elektronik.UI.Windows;
using UniRx;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginWindowsManager : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private Transform Toolbar;
        [SerializeField] private GameObject ButtonPrefab;
        [SerializeField] private WindowsManager WindowsManager;
        [SerializeField] private GameObject WindowPrefab;
        [SerializeField] private Transform Canvas;

        #endregion

        private readonly Dictionary<IElektronikPlugin, (ToolbarButton, Window)> _gameObjects =
                new Dictionary<IElektronikPlugin, (ToolbarButton, Window)>();

        public void RegisterPlugin(IElektronikPlugin plugin)
        {
            if (plugin.Settings == null) return;

            var windowGO = Instantiate(WindowPrefab, Canvas);
            var window = windowGO.GetComponent<Window>();
            window.TitleLabel.text = plugin.DisplayName;
            window.SetManager(WindowsManager);
            
            var go = Instantiate(ButtonPrefab, Toolbar);
            var button = go.GetComponent<ToolbarButton>();
            button.Setup(plugin.Logo, plugin.DisplayName);
            button.OnClick.Subscribe(_ => window.Show());

            var generator = windowGO.GetComponent<SettingsGenerator>();
            generator.Generate(plugin.Settings);
            
            _gameObjects.Add(plugin, (button, window));
        }

        public void UnregisterPlugin(IElektronikPlugin plugin)
        {
            if (!_gameObjects.ContainsKey(plugin)) return;
            var (button, window) = _gameObjects[plugin];
            Destroy(button.gameObject);
            Destroy(window.gameObject);
            _gameObjects.Remove(plugin);
        }
    }
}