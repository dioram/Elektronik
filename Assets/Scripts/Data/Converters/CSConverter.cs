using UnityEngine;

namespace Elektronik.Data.Converters
{
    public abstract class CSConverter : MonoBehaviour, ICSConverter
    {
        public abstract void Convert(ref Vector3 pos, ref Quaternion rot);
        public abstract void Convert(ref Vector3 pos);
        public abstract Vector3 Convert(Vector3 pos);
        public abstract Quaternion Convert(Quaternion rot);
        public abstract (Vector3 pos, Quaternion rot) Convert(Vector3 pos, Quaternion rot);
        public abstract void ConvertBack(ref Vector3 pos);
        public abstract void ConvertBack(ref Vector3 pos, ref Quaternion rot);
        public abstract void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale);
    }
}
