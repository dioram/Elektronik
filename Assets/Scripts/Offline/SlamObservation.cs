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

        public SlamObservation Clone()
        {
            SlamObservation observation = new SlamObservation()
            {
                Position = Position,
                Orientation = Orientation,
                IsRemoved = IsRemoved,
                Color = Color,
                Statistics1 = Statistics1,
                Statistics2 = Statistics2,
                Statistics3 = Statistics3,
                Statistics4 = Statistics4,
            };
            return observation;
        }
    }
}
