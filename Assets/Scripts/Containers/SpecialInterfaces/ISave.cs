using System;
using Elektronik.Data;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface ISave
    {
        public void RequestSaving();
        event Action<ISourceTreeNode> SaveMe;
    }
}