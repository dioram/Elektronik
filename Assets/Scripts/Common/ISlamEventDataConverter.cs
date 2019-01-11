using Elektronik.Common.Events;

namespace Elektronik.Common
{
    public interface ISlamEventDataConverter
    {
        void Convert(ref ISlamEvent srcEvent);
    }
}