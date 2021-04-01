using System.IO;
using Elektronik.Clouds;
using Elektronik.Data.PackageObjects;
using Elektronik.Renderers;
using Elektronik.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Window))]
    public class ObservationViewer : MonoBehaviour, IDataRenderer<DataComponent<SlamObservation>>
    {
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        #region Unity events

        private void Awake()
        {
            Image.texture = Texture2D.whiteTexture;
            Image.gameObject.SetActive(false);
            Window = GetComponent<Window>();
        }

        private void Update()
        {
            SetData();
        }

        #endregion

        #region IDataRenderer
        
        public bool IsShowing => gameObject.activeSelf;

        public void Render(DataComponent<SlamObservation> data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                gameObject.SetActive(true);
                _observation = data;
                Window.Title = $"Observation #{data.Data.Id}";
                SetData();
            });
        }

        public void Clear()
        {
        }

        #endregion

        #region Private
        
        [SerializeField] private RawImage Image;
        [SerializeField] private TMP_Text Message;
        [SerializeField] private Window Window;

        private string _currentFileName;
        private DataComponent<SlamObservation> _observation;

        private void SetData()
        {
            Message.text = _observation.Data.Message;
            Message.gameObject.SetActive(!string.IsNullOrEmpty(Message.text));

            if (_currentFileName == _observation.Data.FileName) return;
            _currentFileName = _observation.Data.FileName;
            if (File.Exists(_currentFileName))
            {
                Texture2D texture = new Texture2D(1024, 1024);
                texture.LoadImage(File.ReadAllBytes(_currentFileName));
                Image.texture = texture;
                Image.gameObject.SetActive(true);
            }
            else
            {
                Image.texture = Texture2D.whiteTexture;
                Image.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}