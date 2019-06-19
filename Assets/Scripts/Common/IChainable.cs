namespace Elektronik.Common
{
    public interface IChainable<T>
    {
        IChainable<T> SetSuccessor(IChainable<T> link);
    }
}
