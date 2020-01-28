using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Parsers.Slam;
using Elektronik.Common.Extensions;

namespace Elektronik.Common.Data.Parsers
{
    public class SlamPackageParser : DataParser
    {
        private IParser parsersChain;
        public SlamPackageParser(ICSConverter converter) : base(converter) 
        {
            parsersChain = new DataParser[]
            {
                new SlamPointsParser(converter),
                new SlamLinesParser(converter),
                new SlamMessageParser(converter),
                new SlamObservationsParser(converter),
            }.BuildChain();
        }

        public override IPackage Parse(byte[] data, int startIdx, ref int offset)
        {
            if ((PackageType)data[startIdx] != PackageType.SLAMPackage)
            {
                return m_successor?.Parse(data, startIdx, ref offset);
            }
            return parsersChain.Parse(data, startIdx, ref offset);
        }
    }
}
