namespace Elektronik.Renderers
{
    public interface IDataRenderer<T>
    {
        void Render(T data);
        void Clear();
    }
}