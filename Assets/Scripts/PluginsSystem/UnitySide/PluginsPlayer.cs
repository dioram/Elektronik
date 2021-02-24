using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsPlayer : MonoBehaviour
    {
        public static readonly ReadOnlyCollection<IElektronikPlugin> Plugins = PluginsLoader.ActivePlugins.AsReadOnly();

        private void Start()
        {
            foreach (var plugin in Plugins)
            {
                // TODO: Send renders and player events
                plugin.Start();
            }
        }

        private void Update()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Update();
            }
        }

        private void OnDestroy()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Stop();
            }
        }
    }
}