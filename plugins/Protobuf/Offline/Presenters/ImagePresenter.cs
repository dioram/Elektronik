using System.IO;
using Elektronik.Presenters;
using Elektronik.Renderers;

namespace Elektronik.Protobuf.Offline.Presenters
{
    public class ImagePresenter : DataPresenter
    {
        private IDataRenderer<byte[]> _renderer;
        private readonly OfflineSettingsBag _settings;

        public ImagePresenter(OfflineSettingsBag settings)
        {
            _settings = settings;
        }

        public override void Clear()
        {
            _renderer.Clear();
            base.Clear();
        }

        public override void Present(object data)
        {
            if (data is Frame frame && _renderer != null)
            {
                var fullPath = Path.Combine(_settings.ImagePath, $"{frame.Timestamp}.png");
                if (File.Exists(fullPath))
                {
                    var currentImage = File.ReadAllBytes(fullPath);
                    _renderer.Render(currentImage);
                }
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