using Elektronik.Commands;
using Elektronik.RosPlugin.Common.Containers;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers
{
    public class ShowCommand<TPresenter, TMessage> : ICommand
    where TPresenter : IPresenter<TMessage>
    {
        private readonly TPresenter _presenter;
        private readonly TMessage _newData;
        private readonly TMessage? _oldData;

        public ShowCommand(TPresenter presenter, TMessage data)
        {
            _presenter = presenter;
            _oldData = _presenter.Current;
            _newData = data;
        }

        public void Execute()
        {
            _presenter.Present(_newData);
        }

        public void UnExecute()
        {
            if (_oldData != null) _presenter.Present(_oldData);
        }
    }
}