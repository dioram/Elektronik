using Elektronik.Offline.Events;

namespace Elektronik.Offline
{
    public interface ISlamEventDataConverter
    {
        void Convert(ref ISlamEvent srcEvent);
    }
}