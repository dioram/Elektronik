using Elektronik.Common.Data.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data.Parsers
{
    public interface IParser
    {
        IPackage Parse(byte[] data, int startIdx, ref int offset);
    }
}
