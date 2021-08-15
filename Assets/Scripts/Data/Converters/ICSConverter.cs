using UnityEngine;

namespace Elektronik.Data.Converters
{
    /// <summary> Translates data between coordinate systems. </summary>
    public interface ICSConverter
    {
        void SetInitTRS(Vector3 pos, Quaternion rot);
        void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale);
        void Convert(ref Vector3 pos, ref Quaternion rot);
        void Convert(ref Vector3 pos);
        Vector3 Convert(Vector3 pos);
        Quaternion Convert(Quaternion rot);
        (Vector3 pos, Quaternion rot) Convert(Vector3 pos, Quaternion rot);

        void ConvertBack(ref Vector3 pos);
        void ConvertBack(ref Vector3 pos, ref Quaternion rot);
    }
}
