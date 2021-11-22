using System;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that camera can follow content of this container. </summary>
    /// <typeparam name="TCloudItem"> Type of cloud items </typeparam>
    public interface IFollowableDataSource<TCloudItem> : IDataSource
            where TCloudItem : struct, ICloudItem
    {
        // TODO: Refactor this? Maybe it should somehow be data consumer function.
        
        public void Follow();
        public void Unfollow();
        
        event Action<IFollowableDataSource<TCloudItem>, ICloudContainer<TCloudItem>, TCloudItem> OnFollowed;
        event Action<ICloudContainer<TCloudItem>, TCloudItem> OnUnfollowed;
        
        bool IsFollowed { get; }
    }
}