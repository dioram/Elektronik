namespace Elektronik.Plugins.Common.Parsing
{
    /// <summary> Interface for handlers in pattern "Chain of responsibility" </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChainable<T>
    {
        /// <summary> Sets next command to perform after this. </summary>
        /// <param name="link"> Next command. </param>
        /// <returns> Successor. </returns>
        IChainable<T>? SetSuccessor(IChainable<T>? link);
    }
}
