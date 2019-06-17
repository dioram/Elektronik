using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class Robot2Unity3dSlamEventConverter : ICSConverter
    {
        public void Convert(ref SlamPackage srcEvent)
        {
            throw new NotImplementedException();
        }

        public void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            throw new NotImplementedException();
        }
    }
}
