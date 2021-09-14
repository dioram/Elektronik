namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for factories for data source plugins. </summary>
    public interface IDataSourcePluginsFactory : IElektronikPluginsFactory
    {
    }

    /// <summary> Interface for factories for data source plugins that are reading data from files. </summary>
    public interface IFileSourcePluginsFactory : IDataSourcePluginsFactory
    {
        public string[] SupportedExtensions { get; }
    }

    /// <summary> Interface for factories for plugins that can read snapshots. </summary>
    public interface ISnapshotReaderPluginsFactory : IFileSourcePluginsFactory
    {
        public void SetFileName(string path);
    }
}