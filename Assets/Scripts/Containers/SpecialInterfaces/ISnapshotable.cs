using Elektronik.PluginsSystem;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface ISnapshotable
    {
        ISnapshotable TakeSnapshot();

        void WriteSnapshot(IDataRecorderPlugin recorder);
    }
}