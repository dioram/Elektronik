namespace Elektronik.Common.Commands
{
    public interface ICommand
    {
        /// <summary> Executes this command. </summary>
        void Execute();
        
        /// <summary> Undo this command. </summary>
        void UnExecute();
    }
}
