using Elektronik.Commands;

namespace Elektronik.Protobuf.Online.Presenters
{
    public class ImageCommand : StatelessCommand
    {
        private readonly RawImagePresenter _presenter;
        private readonly byte[] _imageData;
        
        public ImageCommand(RawImagePresenter presenter, byte[] imageData, ICommand? previous) : base(previous)
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