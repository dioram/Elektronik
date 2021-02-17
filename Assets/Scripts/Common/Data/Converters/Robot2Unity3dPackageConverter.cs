using System;
using UnityEngine;

namespace Elektronik.Common.Data.Converters
{
    public class Robot2Unity3dSlamEventConverter : CSConverter
    {
        public override void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            throw new NotImplementedException();
        }

        public override void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            throw new NotImplementedException();
        }
    }
}
