namespace Elektronik.Renderers
{
    public interface IDataRenderer<T>
    {
        bool IsShowing { get; set; }
        void Render(T data);
        void Clear();
    }
}