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
        public static CloudPoint Empty(int id) => new CloudPoint(id, Vector3.zero, new Color(0, 0, 0, 0));
        public int idx;
        public Vector3 offset;
        public Color color;
        public CloudPoint(int idx, Vector3 offset, Color color)
        {
            this.idx = idx;
            this.offset = offset;
            this.color = color;
        }
    }
}
