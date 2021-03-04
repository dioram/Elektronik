using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Elektronik.Clouds;
using Elektronik.Data.Converters;
using Elektronik.Offline;
using Elektronik.Renderers;
using Elektronik.UI;
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

        private void Start()
        {
            var cloudRenderers = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .Any(i => i.GetGenericTypeDefinition() == typeof(ICloudRenderer<>)))
                    .SelectMany(t => Renderers.GetComponentsInChildren(t))
                    .ToArray();
            var dataRenderers = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .Any(i => i.GetGenericTypeDefinition() == typeof(IDataRenderer<>)))
                    .SelectMany(t => Renderers.GetComponentsInChildren(t))
                    .ToArray();
            foreach (var dataSource in Plugins.OfType<IDataSource>())
            {
                foreach (var r in cloudRenderers)
                {
                    dataSource.Data.SetRenderer(r);
                }
                foreach (var r in dataRenderers)
                {
                    dataSource.PresentersChain?.SetRenderer(r);
                }

                dataSource.Converter = Converter;
                var treeElement = Instantiate(ContainerTreePrefab, TreeView).GetComponent<ContainerTreeElement>();
                treeElement.Node = dataSource.Data;
            }

            foreach (var dataSource in Plugins.OfType<IDataSourceOffline>())
            {
                PlayerEvents.SetDataSource(dataSource);
            }

            foreach (var plugin in Plugins)
            {
                plugin.Start();
            }
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
            foreach (var dataSourceOnline in PluginsLoader.ActivePlugins.OfType<IDataSourceOnline>())
            {
                dataSourceOnline.Data.Clear();
            }
            foreach (var dataSourceOffline in PluginsLoader.ActivePlugins.OfType<IDataSourceOffline>())
            {
                dataSourceOffline.StopPlaying();
            }
        }
    }
}