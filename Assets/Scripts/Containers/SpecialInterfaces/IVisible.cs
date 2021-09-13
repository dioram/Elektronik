using System;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface IVisible
    {
        public bool IsVisible { get; set; }

        public event Action<bool> OnVisibleChanged; 
        
        public bool ShowButton { get; }
    }
}