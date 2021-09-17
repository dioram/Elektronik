using System;

namespace Elektronik.DataSources.Containers.SpecialInterfaces
{
    public interface IRemovable
    {
        public void RemoveSelf();
        public event Action OnRemoved;
    }
}