using Elektronik.Data.Converters;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class FakeConverter: ICSConverter
    {
        public void SetInitTRS(Vector3 pos, Quaternion rot)
        { }

        public void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale)
        { }

        public void Convert(ref Vector3 pos, ref Quaternion rot)
        { }

        public void Convert(ref Vector3 pos)
        { }

        public Vector3 Convert(Vector3 pos)
        {
            return pos;
        }

        public Quaternion Convert(Quaternion rot)
        {
            return rot;
        }

        public (Vector3 pos, Quaternion rot) Convert(Vector3 pos, Quaternion rot)
        {
            return (pos, rot);
        }

        public void ConvertBack(ref Vector3 pos)
        { }

        public void ConvertBack(ref Vector3 pos, ref Quaternion rot)
        { }
    }
}