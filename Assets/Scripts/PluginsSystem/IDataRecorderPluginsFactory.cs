namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for factories for plugins that can record data. </summary>
    public interface IDataRecorderPluginsFactory : IElektronikPluginsFactory
    {
    }

    public interface IFileRecorderPluginsFactory : IDataRecorderPluginsFactory
    {
        string Extension { get; }
        string Filename { get; set; }
    }

    public interface ICustomRecorderPluginsFactory : IDataRecorderPluginsFactory
    {
    }
}