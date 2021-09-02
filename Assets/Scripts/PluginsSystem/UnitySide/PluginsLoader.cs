using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elektronik.UI.Localization;
using Debug = UnityEngine.Debug;

namespace Elektronik.PluginsSystem.UnitySide
{
    public static class PluginsLoader
    {
        public static readonly Lazy<List<IElektronikPluginsFactory>> Plugins =
                new Lazy<List<IElektronikPluginsFactory>>(LoadPlugins);

        public static readonly List<IElektronikPluginsFactory> ActivePlugins = new List<IElektronikPluginsFactory>();

        public static void EnablePlugin(IElektronikPluginsFactory plugin)
        {
            if (!ActivePlugins.Contains(plugin)) ActivePlugins.Add(plugin);
        }

        public static void DisablePlugin(IElektronikPluginsFactory plugin)
        {
            if (ActivePlugins.Contains(plugin)) ActivePlugins.Remove(plugin);
        }

        #region Private

        private static List<IElektronikPluginsFactory> LoadPlugins()
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
                        res.AddRange(Assembly.LoadFrom(file)
                                             .GetTypes()
                                             .Where(p => typeof(IElektronikPluginsFactory).IsAssignableFrom(p) &&
                                                            p.IsClass && !p.IsAbstract)
                                             .Select(InstantiatePlugin<IElektronikPluginsFactory>)
                                             .Where(p => p != null));
                        TextLocalizationExtender.ImportTranslations(Path.Combine(Path.GetDirectoryName(file)!,
                                                                        @"../data/translations.csv"));
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

        private static T InstantiatePlugin<T>(Type t) where T : class
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