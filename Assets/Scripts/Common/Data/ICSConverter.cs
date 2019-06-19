using UnityEngine;

namespace Elektronik.Common.Data
{
    public interface ICSConverter
    {
        void Convert(ref Vector3 pos, ref Quaternion rot);
    }
}
