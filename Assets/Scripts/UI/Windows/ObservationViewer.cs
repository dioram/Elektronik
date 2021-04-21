using System.Collections;
using System.Collections.Generic;
using System.IO;
using Elektronik.Clouds;
using Elektronik.Data.PackageObjects;
using Elektronik.Renderers;
using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Windows
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
            Image.transform.parent.gameObject.SetActive(false);
            Window = GetComponent<Window>();
        }

        private void OnEnable()
        {
            StartCoroutine(UpdatePicture());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region IDataRenderer
        
        public bool IsShowing
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public void Render(DataComponent<SlamObservation> data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                gameObject.SetActive(true);
                _observation = data;
                Window.TitleLabel.SetLocalizedText("Observation #{0}", new List<object>{data.Data.Id});
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
        [SerializeField] private GameObject TextView;
        [SerializeField] private AspectRatioFitter Fitter;

        private DataComponent<SlamObservation> _observation;

        private IEnumerator UpdatePicture()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                SetData();
            }
        }

        private void SetData()
        {
            Message.text = _observation.Data.Message;
            TextView.SetActive(!string.IsNullOrEmpty(Message.text));

            if (File.Exists(_observation.Data.FileName))
            {
                var texture = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
                texture.LoadImage(File.ReadAllBytes(_observation.Data.FileName));
                texture.filterMode = FilterMode.Trilinear;
                Image.texture = texture;
                Image.transform.parent.gameObject.SetActive(true);
                Fitter.aspectRatio = texture.width / (float) texture.height;
            }
            else
            {
                Image.texture = Texture2D.whiteTexture;
                Image.transform.parent.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}