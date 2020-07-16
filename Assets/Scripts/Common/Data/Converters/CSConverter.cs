using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data.Converters
{
    public abstract class CSConverter : MonoBehaviour, ICSConverter
    {
        public abstract void Convert(ref Vector3 pos, ref Quaternion rot);

        public abstract void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale);
    }
}
