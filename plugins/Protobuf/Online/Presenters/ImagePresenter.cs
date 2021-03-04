using Elektronik.Presenters;
using Elektronik.Renderers;

namespace Elektronik.Protobuf.Online.Presenters
{
    public class ImagePresenter : DataPresenter
    {
        private IDataRenderer<byte[]> _renderer;

        public override void Clear()
        {
            _renderer.Clear();
            base.Clear();
        }

        public override void Present(object data)
        {
            if (data is byte[] frame)
            {
                _renderer?.Render(frame);
            }
            base.Present(data);
        }

        public override void SetRenderer(object dataRenderer)
        {
            if (dataRenderer is IDataRenderer<byte[]> renderer)
            {
                _renderer = renderer;
            }
            base.SetRenderer(dataRenderer);
        }
    }
}