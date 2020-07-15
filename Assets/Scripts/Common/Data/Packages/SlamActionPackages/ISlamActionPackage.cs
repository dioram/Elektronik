using Elektronik.Common.Data.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data.Packages
{
    public interface ISlamActionPackage : IPackage
    {
        ObjectType ObjectType { get; }
        ActionType ActionType { get; }
    }
}
