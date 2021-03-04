using System.IO;
using Elektronik.Clouds;
using Elektronik.Data.PackageObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class ObservationViewer : MonoBehaviour
    {
        public RawImage m_image;
        public Text m_message;

        private DataComponent<SlamObservation> _observation;
        private string _currentFileName;

        private void Start()
        {
            m_image.texture = Texture2D.whiteTexture;
        }

        private void Update()
        {
            SetData();
        }

        public void ShowObservation(DataComponent<SlamObservation> observation)
        {
            gameObject.SetActive(true);
            _observation = observation;
        }

        private void SetData()
        {
            m_message.text = _observation.Data.Message;

            if (_currentFileName == _observation.Data.FileName) return;
            _currentFileName = _observation.Data.FileName;
            if (File.Exists(_currentFileName))
            {
                Texture2D texture = new Texture2D(1024, 1024);
                texture.LoadImage(File.ReadAllBytes(_currentFileName));
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