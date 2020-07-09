using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public struct CloudPoint
    {
        public readonly int idx;
        public readonly Vector3 offset;
        public readonly Color color;
        public CloudPoint(int idx, Vector3 offset, Color color)
        {
            this.idx = idx;
            this.offset = offset;
            this.color = color;
        }
    }
}
