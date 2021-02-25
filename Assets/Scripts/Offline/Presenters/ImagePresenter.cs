using System.IO;
using Elektronik.Common.Cameras;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Presenters;
using Elektronik.Common.Settings;
using Elektronik.Offline.Settings;

namespace Elektronik.Offline.Presenters
{
    public class ImagePresenter : RepaintablePackagePresenter
    {
        public CameraImageRenderer Target;
        private byte[] _currentImage;

        public override void Present(PacketPb package)
        {
            // TODO: fix back
            var fullPath = //Path.Combine(SettingsBag.GetCurrent<OfflineSettingsBag>().ImagePath,
                    $"{package.Timestamp}.png";//);
            if (!File.Exists(fullPath)) return;

            _currentImage = File.ReadAllBytes(fullPath);
            if (Successor != null) Successor.Present(package);
        }

        public override void Repaint()
        {
            if (_currentImage == null) return;
            Target.DrawImage(_currentImage);
        }

        public override void Clear()
        {
            Target.ClearImage();
        }
    }
}