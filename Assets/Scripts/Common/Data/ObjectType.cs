using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data
{
    public enum ObjectType : byte
    {
        Point = 0,
        Observation = 1,
        Line = 2,
        Message = 3,
    }
}
