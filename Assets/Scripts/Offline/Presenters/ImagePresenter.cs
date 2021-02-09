using System.IO;
using Elektronik.Common.Cameras;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Presenters;
using Elektronik.Common.Settings;
using Elektronik.Offline.Settings;
using UnityEngine;

namespace Elektronik.Offline.Presenters
{
    public class ImagePresenter : RepaintablePackagePresenter
    {
        public CameraImageRenderer Target;
        private byte[] m_currentImage;
        
        public override void Present(PacketPb package)
        {
            var fullPath = Path.Combine(SettingsBag.Current[SettingName.ImagePath].As<string>(), $"{package.Timestamp}.png");
            if (!File.Exists(fullPath)) return;
            
            m_currentImage = File.ReadAllBytes(fullPath);
            m_successor?.Present(package);
        }

        public override void Repaint()
        {
            if (m_currentImage == null) return;
            Target.DrawImage(m_currentImage);
        }

        public override void Clear()
        {
            Target.ClearImage();
        }
    }
}