namespace Elektronik.Common.Containers
{
    public interface ILinesContainer<T> : IContainer<T>
    {
        bool Exists(int id1, int id2);
        T Get(int id1, int id2);
        bool TryGet(int id1, int id2, out T line);
        bool Remove(int id1, int id2);
    }
}
