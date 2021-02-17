namespace Elektronik.Common.Commands
{
    public interface ICommand
    {
        void Execute();
        void UnExecute();
    }
}
