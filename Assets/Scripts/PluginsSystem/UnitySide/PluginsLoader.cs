using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public static class PluginsLoader
    {
        public static readonly List<IElektronikPlugin> Plugins = new List<IElektronikPlugin>();
        public static readonly List<IElektronikPlugin> ActivePlugins = new List<IElektronikPlugin>();

        public static void EnablePlugin(IElektronikPlugin plugin)
        {
            if (!ActivePlugins.Contains(plugin)) ActivePlugins.Add(plugin);
        }

        public static void DisablePlugin(IElektronikPlugin plugin)
        {
            if (ActivePlugins.Contains(plugin)) ActivePlugins.Remove(plugin);
        }

        static PluginsLoader()
        {            
            var pluginsDir = Path.Combine(Directory.GetCurrentDirectory(), @".\Elektronik tools_Data\Managed");
            foreach (var file in Directory.GetFiles(pluginsDir, "*.dll"))
            {
                Debug.LogError(file);
                try
                {
                    Plugins.AddRange(Assembly.LoadFrom(file)
                                             .GetTypes()
                                             .Where(p => typeof(IElektronikPlugin).IsAssignableFrom(p) && p.IsClass &&
                                                            !p.IsAbstract)
                                             .Select(InstantiatePlugin<IElektronikPlugin>)
                                             .Where(p => p != null));
                }
                catch (Exception e)
                {
                    Debug.LogError(file);
                }
            }
        }

        private static T InstantiatePlugin<T>(Type t) where T : class
        {
            try
            {
                return (T) Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                Debug.LogError($"Plugin initialisation error. {t} - {e.Message}");
            }

            return null;
        }
    }
}