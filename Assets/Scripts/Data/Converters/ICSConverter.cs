using UnityEngine;

namespace Elektronik.Data.Converters
{
    /// <summary> Translates data between coordinate systems. </summary>
    public interface ICSConverter
    {
        void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale);
        void Convert(ref Vector3 pos, ref Quaternion rot);
    }
}
