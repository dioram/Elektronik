using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elektronik.Settings;
using Elektronik.UI.Localization;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsLoader : MonoBehaviour
    {
        public static readonly List<IElektronikPluginsFactory> PluginFactories = new List<IElektronikPluginsFactory>();

        private static readonly Dictionary<IElektronikPluginsFactory, string> PathsToDlls =
                new Dictionary<IElektronikPluginsFactory, string>();

        private void Awake()
        {
            SettingsRepository.Path = Application.persistentDataPath;
            var pluginsLoading = Observable.Start(LoadPluginFactories);
            Observable.WhenAll(pluginsLoading)
                    .ObserveOnMainThread()
                    .Do(_ => LoadLogos())
                    .Do(_ => Debug.Log($"Loaded {PluginFactories.Count} plugins"))
                    .Do(_ => SceneManager.LoadScene("Scenes/Main"))
                    .Subscribe()
                    .AddTo(this);
        }

        #region Private

        private static void LoadPluginFactories()
        {
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
                        var factories = LoadFromFile(file);
                        PluginFactories.AddRange(factories);
                        foreach (var factory in factories)
                        {
                            PathsToDlls[factory] = file;
                        }
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

            TextLocalizationExtender.ImportTranslations(Path.Combine(Path.GetDirectoryName(path)!,
                                                                     @"../data/translations.csv"));
            return factories;
        }

        private void LoadLogos()
        {
            foreach (var factory in PluginFactories)
            {
                var path = Path.Combine(Path.GetDirectoryName(PathsToDlls[factory])!,
                                        $@"../data/{factory.DisplayName}_Logo.png");
                if (!File.Exists(path)) continue;
                factory.Logo = new Texture2D(1, 1);
                factory.Logo.LoadImage(File.ReadAllBytes(path));
            }
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