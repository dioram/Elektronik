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
        public GameObjectsContainer<SlamObservation> Observations;

        private int _observationId;
        private string _currentFileName;

        private void Start()
        {
            m_image.texture = Texture2D.whiteTexture;
        }

        private void Update()
        {
            SetData();
        }

        public void ShowObservation(int observation)
        {
            gameObject.SetActive(true);
            _observationId = observation;
        }

        void SetData()
        {
            SlamObservation observation = Observations[_observationId];
            m_message.text = observation.Message;

            if (_currentFileName == observation.FileName) return;
            _currentFileName = observation.FileName;
            var path = ModeSelector.Mode == Mode.Online
                    ? Directory.GetCurrentDirectory()
                    : SettingsBag.GetCurrent<OfflineSettingsBag>().ImagePath;
            path = Path.Combine(path, observation.FileName);
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