using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data
{
    public class TrackingPackage : IPackage
    {
        public PackageType Type => PackageType.TrackingPackage;

        public int Timestamp => throw new NotImplementedException();

        public bool IsKey => throw new NotImplementedException();
    }
}
