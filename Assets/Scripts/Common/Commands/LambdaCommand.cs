using System;

namespace Elektronik.Common.Commands
{
    public class LambdaCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action _unexecute;

        public LambdaCommand(Action execute, Action unexecute)
        {
            _execute = execute;
            _unexecute = unexecute;
        }

        public void Execute() => _execute();

        public void UnExecute() => _unexecute();
    }
}
