using System;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that this container can be removed from tree by it self. </summary>
    public interface IRemovableDataSource : IDataSource
    {
        /// <summary> Remove this container from tree. </summary>
        public void RemoveSelf();
        
        /// <summary> This event will be raised when container was removed. </summary>
        public event Action OnRemoved;
    }
}