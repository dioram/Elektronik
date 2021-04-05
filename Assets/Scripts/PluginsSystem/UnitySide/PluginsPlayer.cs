using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Clouds;
using Elektronik.Data.Converters;
using Elektronik.Offline;
using Elektronik.UI;
using Elektronik.UI.Windows;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsPlayer : MonoBehaviour
    {
        public static readonly ReadOnlyCollection<IElektronikPlugin> Plugins = PluginsLoader.ActivePlugins.AsReadOnly();
        public GameObject Renderers;
        public CSConverter Converter;
        public PlayerEventsManager PlayerEvents;
        public GameObject ContainerTreePrefab;
        public RectTransform TreeView;
        public GameObject ScreenLocker;

        public event Action PluginsStarted; 

        private readonly List<Thread> _startupThreads = new List<Thread>();

        private void Start()
        {
            ScreenLocker.SetActive(true);
            
            var cloudRenderers = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .Any(i => i.GetGenericTypeDefinition() == typeof(ICloudRenderer<>)))
                    .SelectMany(t => Renderers.GetComponentsInChildren(t))
                    .Concat(new []{Renderers.transform.Find("Windows").GetComponent<WindowsManager>()})
                    .ToArray();
            
            foreach (var dataSource in Plugins.OfType<IDataSourcePlugin>())
            {
                foreach (var r in cloudRenderers)
                {
                    dataSource.Data.SetRenderer(r);
                }

                dataSource.Converter = Converter;
                var treeElement = Instantiate(ContainerTreePrefab, TreeView).GetComponent<SourceTreeElement>();
                treeElement.Node = dataSource.Data;
                if (Plugins.OfType<IDataSourcePlugin>().Count() == 1)
                {
                    treeElement.ChangeState();
                }
            }

            foreach (var dataSource in Plugins.OfType<IDataSourcePluginOffline>())
            {
                PlayerEvents.SetDataSource(dataSource);
            }

            foreach (var plugin in Plugins)
            {
                var thread = new Thread(() => plugin.Start());
                thread.Start();
                _startupThreads.Add(thread);
            }

            PluginsStarted += () => ScreenLocker.SetActive(false);

            Task.Run(() =>
            {
                foreach (var thread in _startupThreads)
                {
                    thread.Join();
                }

                MainThreadInvoker.Instance.Enqueue(() => PluginsStarted?.Invoke());
            });
        }

        private void Update()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Update(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Stop();
            }
        }

        public void ClearMap()
        {
            foreach (var dataSourceOnline in PluginsLoader.ActivePlugins.OfType<IDataSourcePluginOnline>())
            {
                dataSourceOnline.Data.Clear();
            }
            foreach (var dataSourceOffline in PluginsLoader.ActivePlugins.OfType<IDataSourcePluginOffline>())
            {
                dataSourceOffline.StopPlaying();
            }
        }
    }
}