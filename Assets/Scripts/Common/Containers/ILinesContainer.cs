using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public interface IConnectionsContainer<T> : IRepaintableContainer<T>
    {
        SlamLine2[] this[int id] { get; }

        SlamLine2[] this[SlamPoint pt] { get; }

        void Remove(int id);
        void Update(SlamPoint obj);
        void Update(IEnumerable<SlamPoint> objs);
    }
}
