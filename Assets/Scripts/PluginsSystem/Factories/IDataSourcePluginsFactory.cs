namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for factories for data source plugins. </summary>
    public interface IDataSourcePluginsFactory : IElektronikPluginsFactory
    {
    }

    /// <summary> Interface for factories for plugins that can read snapshots. </summary>
    public interface ISnapshotReaderPluginsFactory : IDataSourcePluginsFactory
    {
        public string[] SupportedExtensions { get; }
        public void SetFileName(string path);
    }
}