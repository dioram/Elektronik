using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.ProtobufPlugin.Offline;
using Elektronik.ProtobufPlugin.Online;
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
            Plugins.AddRange(AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .SelectMany(s => s.GetTypes())
                                     .Where(p => typeof(IElektronikPlugin).IsAssignableFrom(p) && p.IsClass &&
                                                    !p.IsAbstract)
                                     .Select(InstantiatePlugin<IElektronikPlugin>)
                                     .Where(p => p != null)
                                     .ToList());

            //TODO: remove after tests
            // ActivePlugins.Add(Plugins.OfType<ProtobufGrpcServer>().First());
            // ActivePlugins.Add(Plugins.OfType<ProtobufFilePlayer>().First());
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