using System;

namespace Elektronik.DataSources.SpecialInterfaces
{
    public interface IVisible
    {
        public bool IsVisible { get; set; }

        public event Action<bool> OnVisibleChanged; 
        
        public bool ShowButton { get; }
    }
}