using System;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface IRemovable
    {
        public void RemoveSelf();
        public event Action OnRemoved;
    }
}