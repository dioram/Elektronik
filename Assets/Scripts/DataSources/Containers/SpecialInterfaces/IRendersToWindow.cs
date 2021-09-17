using Elektronik.UI.Windows;

namespace Elektronik.DataSources.Containers.SpecialInterfaces
{
    public interface IRendersToWindow
    {
        public Window Window { get; }
        
        public string Title { get; set; }
    }
}