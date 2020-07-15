using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data.Converters
{
    public class EmptyPackageConverter : ICSConverter
    {
        public void Convert(ref Vector3 pos, ref Quaternion rot)
        {
        }
    }
}
