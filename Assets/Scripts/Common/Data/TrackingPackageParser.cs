using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data
{
    public class TrackingPackageParser : DataParser
    {
        public TrackingPackageParser(ICSConverter converter) : base(converter) { }

        private void Convert(ref TrackingPackage pkg)
        {

        }

        public override int Parse(byte[] data, int startIdx, out IPackage result)
        {
            result = null;
            if ((PackageType)data[startIdx] != PackageType.TrackingPackage)
            {
                return m_successor?.Parse(data, startIdx, out result) ?? 0;
            }
            var trackingPackage = new TrackingPackage();
            Convert(ref trackingPackage);
            throw new NotImplementedException();
        }
    }
}
