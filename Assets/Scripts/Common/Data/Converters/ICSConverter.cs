using UnityEngine;

namespace Elektronik.Common.Data.Converters
{
    public interface ICSConverter
    {
        void Convert(ref Vector3 pos, ref Quaternion rot);
    }
}
