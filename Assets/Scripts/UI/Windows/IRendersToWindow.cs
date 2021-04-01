namespace Elektronik.UI.Windows
{
    public interface IRendersToWindow
    {
        public Window Window { get; }
        
        public string Title { get; set; }
    }
}