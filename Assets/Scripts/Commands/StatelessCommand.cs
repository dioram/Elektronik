using JetBrains.Annotations;

namespace Elektronik.Commands
{
    public abstract class StatelessCommand : ICommand
    {
        [CanBeNull] private readonly ICommand _previous;

        protected StatelessCommand([CanBeNull] ICommand previous)
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