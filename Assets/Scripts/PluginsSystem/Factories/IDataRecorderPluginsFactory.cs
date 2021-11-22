namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for plugin factories that can record data. </summary>
    public interface IDataRecorderPluginsFactory : IElektronikPluginsFactory
    {
    }

    /// <summary> Interface for plugin factories that can record data to file. </summary>
    public interface IFileRecorderPluginsFactory : IDataRecorderPluginsFactory
    {
        /// <summary> Supported extension. </summary>
        string Extension { get; }
        string Filename { get; set; }
    }
}