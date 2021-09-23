namespace Elektronik.Plugins.Common.Commands
{
    /// <summary> Interface for command in pattern "Chain of command" </summary>
    public interface ICommand
    {
        /// <summary> Executes this command. </summary>
        void Execute();
        
        /// <summary> Undo this command. </summary>
        void UnExecute();
    }
}
