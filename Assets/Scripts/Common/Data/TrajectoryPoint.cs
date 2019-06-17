using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public struct TrajectoryPoint
    {
        public int id;
        public int trajectoryId;
        public Color color;
        public Vector3 position;
        public Quaternion rotation;
        public string msg;
    }
}
