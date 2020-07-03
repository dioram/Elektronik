using System;
using System.Collections.Generic;
using System.Text;

namespace Elektronik.Common.Data.Pb
{
    public partial class ObservationPb
    {
        public static implicit operator PointPb(ObservationPb obs) => obs.Point;
    }
}
