namespace Elektronik.Renderers
{
    public interface IDataRenderer<T> : ISourceRenderer
    {
        bool IsShowing { get; set; }
        void Render(T data);
        void Clear();
    }
}