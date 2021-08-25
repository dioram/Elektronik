using System.IO;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline.Presenters
{
    public class FileImagePresenter : ImagePresenter<Frame>
    {
        private readonly string _imagePath;

        public FileImagePresenter(string displayName, string imagePath) : base(displayName)
        {
            _imagePath = imagePath;
        }

        public override void Present(Frame frame)
        {
            if (Renderer == null) return;
            var fullPath = Path.Combine(_imagePath ?? "", $"{frame.Timestamp}.png");
            if (!File.Exists(fullPath)) return;
            var currentImage = File.ReadAllBytes(fullPath);
            Renderer.Render(currentImage);
        }
    }
}