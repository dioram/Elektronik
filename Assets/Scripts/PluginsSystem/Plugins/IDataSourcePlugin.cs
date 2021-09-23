using Elektronik.DataSources;

namespace Elektronik.PluginsSystem
{
    public interface IDataSourcePlugin : IElektronikPlugin
    {
        /// <summary> Root element of containers with data. </summary>
        ISourceTreeNode Data { get; }
    }
}