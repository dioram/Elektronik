using System;
using System.IO;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elektronik
{
    public class CommandLineStarter : MonoBehaviour
    {
        private void Awake()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2 || !File.Exists(args[1])) return;
            var extension = Path.GetExtension(args[1]);
            var plugin = PluginsLoader.Plugins
                    .Value
                    .OfType<IDataSourcePluginOffline>()
                    .First(p => p.SupportedExtensions.Contains(extension));
            ModeSelector.Mode = Mode.Offline;
            PluginsLoader.ActivePlugins.Add(plugin);
            SceneManager.LoadScene("Scenes/Empty", LoadSceneMode.Single);
        }
    }
}