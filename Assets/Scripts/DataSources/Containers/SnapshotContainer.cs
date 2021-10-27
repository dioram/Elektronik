using System;
using System.Collections.Generic;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    public class SnapshotContainer : VirtualDataSource, IRemovableDataSource, ISavableDataSource
    {
        public SnapshotContainer(string displayName, IList<IDataSource> children) : base(displayName, children)
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