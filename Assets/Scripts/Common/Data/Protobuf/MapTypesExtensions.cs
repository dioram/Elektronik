using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx.Operators;
using UnityEngine;

namespace Elektronik.Common.Data.Pb
{
    public partial class Vector3Pb
    {
        public static implicit operator Vector3(Vector3Pb v) 
            => v != null ? new Vector3((float)v.X, (float)v.Y, (float)v.Z) : default;
    }
    public partial class Vector4Pb
    {
        public static implicit operator Quaternion(Vector4Pb v)
            => v != null ? new Quaternion((float)v.X, (float)v.Y, (float)v.Z, (float)v.W) : default;
    }
    public partial class ColorPb
    {
        public static implicit operator Color32(ColorPb c)
            => c != null ? new Color32((byte)c.R, (byte)c.G, (byte)c.B, 255) : new Color32(0, 0, 0, 255);
        public static implicit operator Color(ColorPb c) 
            => (Color32)c;
    }
    public partial class PointPb
    {
        public static implicit operator SlamPoint(PointPb p)
            => p != null ? new SlamPoint() { id = p.id_, position = p.position_, color = p.color_, message = p.message_ } : default;
    }
    public partial class LinePb
    {
        public static implicit operator SlamLine(LinePb c)
            => c != null ? new SlamLine(c.pt1_, c.pt2_) : default;
    }

    public partial class ObservationPb
    {
        public partial class Types
        {
            public partial class Stats
            {
                public static implicit operator SlamObservation.Stats(Stats s)
                    => default; // TODO: make statistics
            }
        }

        public static implicit operator SlamObservation(ObservationPb o)
            => o != null ? new SlamObservation(o.point_, o.orientation_, o.stats_) : default;
    }
}
