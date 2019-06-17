using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data
{
    public interface IDataParser
    {
        int Parse(byte[] data, int startIdx, out IList values);
    }
}
