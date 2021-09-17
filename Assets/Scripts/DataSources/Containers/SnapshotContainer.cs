using System;
using System.Collections.Generic;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    public class SnapshotContainer : VirtualSource, IRemovable, ISave
    {
        public SnapshotContainer(string displayName, IList<ISourceTreeNode> children) : base(displayName, children)
        { }
        
        #region IRemovable
        
        public void RemoveSelf()
        {
            Clear();
            OnRemoved?.Invoke();
        }
        
        public event Action OnRemoved;
        
        #endregion
    }
}