using Elektronik.Offline;

namespace Elektronik.PluginsSystem
{
    public interface IDataSourceOffline : IDataSource
    {
        IPlayerEvents PlayerEvents { get; set; }

        int AmountOfFrames { get; }
    }
}