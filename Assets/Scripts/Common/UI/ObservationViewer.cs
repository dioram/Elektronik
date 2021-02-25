using System.IO;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Settings;
using Elektronik.Offline.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class ObservationViewer : MonoBehaviour
    {
        public RawImage m_image;
        public Text m_message;

        private SlamObservation _observation;
        private string _currentFileName;

        private void Start()
        {
            m_image.texture = Texture2D.whiteTexture;
        }

        private void Update()
        {
            SetData();
        }

        public void ShowObservation(SlamObservation observation)
        {
            gameObject.SetActive(true);
            _observation = observation;
        }

        private void SetData()
        {
            m_message.text = _observation.Message;

            if (_currentFileName == _observation.FileName) return;
            _currentFileName = _observation.FileName;
            // TODO: Fix back
            // var path = ModeSelector.Mode == Mode.Online
            //         ? Directory.GetCurrentDirectory()
            //         : SettingsBag.GetCurrent<OfflineSettingsBag>().ImagePath;
            var path = @"C:\";//Path.Combine(path, _observation.FileName);
            if (File.Exists(path))
            {
                Texture2D texture = new Texture2D(1024, 1024);
                texture.LoadImage(File.ReadAllBytes(path));
                m_image.texture = texture;
            }
            else
            {
                m_image.texture = Texture2D.whiteTexture;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}