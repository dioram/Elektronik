using System;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface IFollowable<TCloudItem> where TCloudItem : struct, ICloudItem
    {
        public void Follow();
        public void Unfollow();
        
        event Action<IFollowable<TCloudItem>, IContainer<TCloudItem>, TCloudItem> OnFollowed;
        event Action<IContainer<TCloudItem>, TCloudItem> OnUnfollowed;
        
        bool IsFollowed { get; }
    }
}