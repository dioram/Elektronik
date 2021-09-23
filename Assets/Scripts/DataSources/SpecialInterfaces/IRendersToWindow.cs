using Elektronik.UI.Windows;

namespace Elektronik.DataSources.SpecialInterfaces
{
    public interface IRendersToWindow
    {
        public Window Window { get; }
        
        public string Title { get; set; }
    }
}