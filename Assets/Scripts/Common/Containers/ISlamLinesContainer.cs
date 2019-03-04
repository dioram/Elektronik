namespace Elektronik.Common.Containers
{
    public interface ISlamLinesContainer<T> : ISlamContainer<T>
    {
        bool Exists(int id1, int id2);
        T Get(int id1, int id2);
        bool TryGet(int id1, int id2, out T line);
        void Remove(int id1, int id2);
    }
}
