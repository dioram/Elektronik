using System;
using System.Collections.Generic;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Special removable container for snapshots. </summary>
    public class SnapshotContainer : VirtualDataSource, IRemovableDataSource, ISavableDataSource
    {
        public SnapshotContainer(string displayName, IList<IDataSource> children) : base(displayName, children)
        { }
        
        #region IRemovable

        /// <inheritdoc />
        public void RemoveSelf()
        {
            Clear();
            OnRemoved?.Invoke();
        }

        /// <inheritdoc />
        public event Action OnRemoved;
        
        #endregion
    }
}