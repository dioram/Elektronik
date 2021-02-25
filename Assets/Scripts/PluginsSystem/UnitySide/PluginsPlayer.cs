using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data.Converters;
using UnityEngine;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class PluginsPlayer : MonoBehaviour
    {
        public static readonly ReadOnlyCollection<IElektronikPlugin> Plugins = PluginsLoader.ActivePlugins.AsReadOnly();
        public GameObject Renderers;
        public CSConverter Converter;

        private void Start()
        {
            var renderers = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.GetInterfaces()
                                                 .Where(i => i.IsGenericType)
                                                 .Any(i => i.GetGenericTypeDefinition() == typeof(ICloudRenderer<>)))
                                    .SelectMany(t => Renderers.GetComponentsInChildren(t))
                                    .ToArray();
            foreach (var dataSource in Plugins.OfType<IDataSource>())
            {
                foreach (var r in renderers)
                {
                    dataSource.Data.SetRenderer(r);
                }

                dataSource.Converter = Converter;
            }

            foreach (var plugin in Plugins)
            {
                // TODO: Send player events
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

        public void ClearMap()
        {
            foreach (var dataSourceOnline in PluginsLoader.ActivePlugins.OfType<IDataSourceOnline>())
            {
                dataSourceOnline.Data.Clear();
            }
        }
    }
}