using System.IO;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Maps;
using Elektronik.Common.Settings;
using Elektronik.Offline.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class ObservationViewer : MonoBehaviour
    {
        public SlamMap slamMap;
        public RawImage m_image;
        public Text m_message;
        public GameObjectsContainer<SlamObservation> Observations;

        private int m_observationId;
        private string currentFileName;

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
            m_observationId = observation;
        }

        void SetData()
        {
            SlamObservation observation = Observations[m_observationId];
            m_message.text = observation.message;
            
            if (currentFileName == observation.fileName) return;
            
            var path = Path.Combine(SettingsBag.Current[SettingName.ImagePath].As<string>(), observation.fileName);
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