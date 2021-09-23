using System;

namespace Elektronik.DataSources.SpecialInterfaces
{
    public interface IRemovable
    {
        public void RemoveSelf();
        public event Action OnRemoved;
    }
}