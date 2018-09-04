using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamPoint
    {
        public Vector3 Position { get; set; }
        public Color Color { get; set; }
        public bool IsRemoved { get; set; }

        public SlamPoint(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public SlamPoint(Vector3 position, Color color, bool isRemoved) : this(position, color)
        {
            IsRemoved = isRemoved;
        }
    }
}
