using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data
{
    public interface IPackage
    {
        PackageType Type { get; }
        int Timestamp { get; }
        bool IsKey { get; }
    }
}
