using Elektronik.Commands;
using Elektronik.RosPlugin.Common.Containers;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers
{
    public class ShowImageCommand : ICommand
    {
        private readonly ImagePresenter _presenter;
        private readonly ImagePresenter.ImageData _newData;
        private ImagePresenter.ImageData? _oldData;

        public ShowImageCommand(ImagePresenter presenter, ImagePresenter.ImageData data)
        {
            _presenter = presenter;
            _newData = data;
        }

        public void Execute()
        {
            _oldData = _presenter.Current;
            _presenter.Present(_newData);
        }

        public void UnExecute()
        {
            if (_oldData != null) _presenter.Present(_oldData);
        }
    }
}