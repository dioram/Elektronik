using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamObservation
    {
        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }
        public bool IsRemoved { get; set; }
        public Color Color { get; set; }
        public byte Statistics1 { get; set; }
        public byte Statistics2 { get; set; }
        public byte Statistics3 { get; set; }
        public byte Statistics4 { get; set; }
    }
}
