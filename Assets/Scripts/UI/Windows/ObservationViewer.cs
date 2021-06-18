using System.Collections;
using System.IO;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Renderers;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Windows
{
    [RequireComponent(typeof(Window))]
    public class ObservationViewer : MonoBehaviour, IDataRenderer<(IContainer<SlamObservation>, SlamObservation)>
    {
        public int ObservationId => _observation.Id;

        public int ObservationContainer => _container.GetHashCode();

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
            NextButton.OnClickAsObservable().Subscribe(_ => ShowNextObservation());
            PreviousButton.OnClickAsObservable().Subscribe(_ => ShowPreviousObservation());
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

        public void Render((IContainer<SlamObservation>, SlamObservation) data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                gameObject.SetActive(true);
                _container = data.Item1;
                _observation = data.Item2;
                Window.TitleLabel.SetLocalizedText("Observation #{0}", _observation.Id);
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
        [SerializeField] private Button PreviousButton;
        [SerializeField] private Button NextButton;

        private IContainer<SlamObservation> _container;
        private SlamObservation _observation;

        private void ShowNextObservation()
        {
            bool found = false;
            foreach (var observation in _container)
            {
                if (observation.Id == _observation.Id)
                {
                    found = true;
                    continue;
                }

                if (!found) continue;
                _observation = observation;
                SetData();
                Window.TitleLabel.SetLocalizedText("Observation #{0}", _observation.Id);
                return;
            }
        }

        private void ShowPreviousObservation()
        {
            SlamObservation prev = _observation;
            foreach (var observation in _container)
            {
                if (observation.Id == _observation.Id)
                {
                    _observation = prev;
                    SetData();
                    Window.TitleLabel.SetLocalizedText("Observation #{0}", _observation.Id);
                    return;
                }

                prev = observation;
            }
        }

        private IEnumerator UpdatePicture()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                SetData();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void SetData()
        {
            Message.text = _observation.Message;
            TextView.SetActive(!string.IsNullOrEmpty(Message.text));

            if (File.Exists(_observation.FileName))
            {
                var texture = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
                texture.LoadImage(File.ReadAllBytes(_observation.FileName));
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