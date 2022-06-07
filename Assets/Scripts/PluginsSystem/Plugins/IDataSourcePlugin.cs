using Elektronik.DataSources;

namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for all plugins that provides data sources. </summary>
    public interface IDataSourcePlugin : IElektronikPlugin
    {
        /// <summary> Root element of containers with data. </summary>
        IDataSource Data { get; }
    }
}