using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elektronik.UI.Localization;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsLoader : MonoBehaviour
    {
        public static readonly Lazy<List<IElektronikPluginsFactory>> PluginFactories =
                new Lazy<List<IElektronikPluginsFactory>>(LoadPluginFactories);

        private void Awake()
        {
            Debug.Log($"Loaded {PluginFactories.Value.Count} plugins");
        }

        #region Private

        private static List<IElektronikPluginsFactory> LoadPluginFactories()
        {
            var res = new List<IElektronikPluginsFactory>();
            try
            {
                var currentDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) ?? "";
                var pluginsDir = Path.Combine(currentDir, @"Plugins");
                var dlls = Directory.GetDirectories(pluginsDir)
                        .Select(d => Path.Combine(d, "libraries"))
                        .Where(Directory.Exists)
                        .SelectMany(d => Directory.GetFiles(d, "*.dll"));
                foreach (var file in dlls)
                {
                    try
                    {
                        res.AddRange(LoadFromFile(file));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Plugin load error: {file}, {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"PluginsLoader initialized with error: {e.Message}");
            }

            return res;
        }

        private static List<IElektronikPluginsFactory> LoadFromFile(string path)
        {
            var factories = Assembly.LoadFrom(path)
                    .GetTypes()
                    .Where(p => typeof(IElektronikPluginsFactory).IsAssignableFrom(p) &&
                                   p.IsClass && !p.IsAbstract)
                    .Select(InstantiatePluginFactory<IElektronikPluginsFactory>)
                    .Where(p => p != null)
                    .ToList();
            foreach (var factory in factories)
            {
                factory.LoadLogo(Path.Combine(Path.GetDirectoryName(path)!,
                                              $@"../data/{factory.DisplayName}_Logo.png"));
            }

            TextLocalizationExtender.ImportTranslations(Path.Combine(Path.GetDirectoryName(path)!,
                                                                     @"../data/translations.csv"));
            return factories;
        }

        private static T InstantiatePluginFactory<T>(Type t) where T : class
        {
            try
            {
                return (T)Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                Debug.LogError($"Plugin initialisation error. {t} - {e.Message}");
            }

            return null;
        }

        #endregion
    }
}