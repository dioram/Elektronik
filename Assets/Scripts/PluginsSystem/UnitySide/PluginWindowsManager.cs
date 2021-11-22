using System.Collections.Generic;
using Elektronik.UI.Buttons;
using Elektronik.UI.SettingsFields;
using Elektronik.UI.Windows;
using UniRx;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    /// <summary> This class makes windows for plugins which supports it. </summary>
    internal class PluginWindowsManager : MonoBehaviour
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

        /// <summary> Creates window for given plugin if needed. </summary>
        public void RegisterPlugin(IElektronikPlugin plugin)
        {
            if (plugin.Settings == null) return;

            var windowGO = Instantiate(WindowPrefab, Canvas);
            var window = windowGO.GetComponent<Window>();
            window.TitleLabel.text = plugin.DisplayName;
            if (!(plugin.Logo is null))
            {
                window.Icon.sprite = Sprite.Create(plugin.Logo, new Rect(0, 0, plugin.Logo.width, plugin.Logo.height),
                                                   Vector2.one * 0.5f);
            }

            window.SetManager(WindowsManager);

            var go = Instantiate(ButtonPrefab, Toolbar);
            var button = go.GetComponent<ToolbarButton>();
            button.Setup(plugin.Logo, plugin.DisplayName);
            button.OnClickAsObservable.Subscribe(_ => window.Show());

            var generator = windowGO.GetComponent<SettingsFieldsUiGenerator>();
            generator.Generate(plugin.Settings);

            _gameObjects.Add(plugin, (button, window));
        }

        /// <summary> Removes window for given plugin. </summary>
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