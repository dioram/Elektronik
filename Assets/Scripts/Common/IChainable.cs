namespace Elektronik.Common
{
    public interface IChainable<T>
    {
        /// <summary> Sets next command to perform after this. </summary>
        /// <param name="link"> Next command. </param>
        /// <returns> Successor. </returns>
        IChainable<T> SetSuccessor(IChainable<T> link);
    }
}
