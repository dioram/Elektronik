namespace Elektronik.Renderers
{
    public interface IDataRenderer<T>
    {
        bool IsShowing { get; }
        void Render(T data);
        void Clear();
    }
}