using Elektronik.Plugins.Common.Commands;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Online.Presenters
{
    public class ImageCommand : StatelessCommand
    {
        private readonly ImagePresenter _presenter;
        private readonly byte[] _imageData;
        
        public ImageCommand(ImagePresenter presenter, byte[] imageData, ICommand? previous) : base(previous)
        {
            _presenter = presenter;
            _imageData = imageData;
        }

        public override void Execute()
        {
            _presenter.Present(_imageData);
        }
    }
}