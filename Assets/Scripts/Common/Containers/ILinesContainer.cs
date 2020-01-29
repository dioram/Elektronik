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
        SlamLine[] this[int id] { get; }

        SlamLine[] this[SlamPoint pt] { get; }

        void Remove(int id);
        void Remove(IEnumerable<int> ids);
        void Remove(IEnumerable<SlamPoint> pts);
        void Update(SlamPoint obj);
        void Update(IEnumerable<SlamPoint> objs);
    }
}
