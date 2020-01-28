using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public interface IRepaintableContainer<T> : IContainer<T>, IRepaintable
    {
    }
}
