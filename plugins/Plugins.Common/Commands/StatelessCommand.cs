namespace Elektronik.Plugins.Common.Commands
{
    public abstract class StatelessCommand : ICommand
    {
        private readonly ICommand? _previous;

        protected StatelessCommand(ICommand? previous)
        {
            _previous = previous;
        }

        public abstract void Execute();

        public void UnExecute()
        {
            _previous?.Execute();
        }
    }
}