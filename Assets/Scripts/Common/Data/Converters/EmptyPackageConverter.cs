using UnityEngine;

namespace Elektronik.Common.Data.Converters
{
    public class EmptyPackageConverter : CSConverter
    {
        public override void Convert(ref Vector3 pos, ref Quaternion rot)
        {
        }

        public override void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            
        }
    }
}
