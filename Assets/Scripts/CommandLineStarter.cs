using System;
using System.IO;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik
{
    // TODO: Add tests.
    /// <summary> Class for starting Elektronik from command line. </summary>
    internal class CommandLineStarter : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private DataSourcePluginsController DataSourcePluginsController;
        [SerializeField] private Window ConnectionWindow;

        #endregion
        
        private void Start()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length < 2 || !File.Exists(args[1])) return;
            var extension = Path.GetExtension(args[1]);
            var factory = PluginsLoader.PluginFactories
                    .OfType<ISnapshotReaderPluginsFactory>()
                    .FirstOrDefault(p => p.SupportedExtensions.Contains(extension));
            if (factory is null)
            {
                Debug.LogError($"Plugin for opening {args[1]} was not found");
                return;
            }
            
            factory.SetFileName(args[1]);
            DataSourcePluginsController.SetNewSource(factory, false);
            ConnectionWindow.Hide();
        }
    }
}