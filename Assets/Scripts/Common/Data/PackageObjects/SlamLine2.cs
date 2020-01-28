using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamLine2
    {
        public readonly SlamPoint pt1;
        public readonly SlamPoint pt2;
        public SlamLine2(SlamPoint pt1, SlamPoint pt2)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
        }
    }
}
