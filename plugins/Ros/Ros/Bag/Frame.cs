using Elektronik.Plugins.Common.Commands;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class Frame
    {
        public long Timestamp { get; }

        private readonly ICommand _command;
        
        public Frame(long timestamp, ICommand command)
        {
            Timestamp = timestamp;
            _command = command;
        }

        public void Show()
        {
            _command.Execute();
        }

        public void Rewind()
        {
            _command.UnExecute();
        }
    }
}