using System.Collections;
using System.Linq;
using Elektronik.Settings;
using Elektronik.UI.Buttons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour, IPointerDownHandler
    {
        #region Editor fields

        [SerializeField] private Color BaseHeaderColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color HighlightHeaderColor = Color.blue;
        [SerializeField] private ChangingButton MinimizeButton;
        [SerializeField] private bool SavingSettings = false;
        public TMP_Text TitleLabel;
        public float MinHeight = 40;
        public float MinWidth = 80;

        #endregion
        
        public bool IsMinimized { get; private set; }

        public void Show()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(HighlightHeader());
            }
            else
            {
                gameObject.SetActive(true);
            }

            transform.SetAsLastSibling();
            SaveSettings();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            SaveSettings();
        }

        public void Minimize()
        {
            IsMinimized = true;
            var rect = (RectTransform)transform;
            _maximizedHeight = rect.sizeDelta.y;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 42);
            _content.SetActive(false);
            foreach (var edge in _edges)
            {
                edge.enabled = false;
            }

            SaveSettings();
        }

        public void Maximize()
        {
            IsMinimized = false;
            var rect = ((RectTransform)transform);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, _maximizedHeight);
            _content.SetActive(true);
            foreach (var edge in _edges)
            {
                edge.enabled = true;
            }

            SaveSettings();
        }
        
        public void SetManager(WindowsManager manager)
        {
            transform.Find("Header").GetComponent<HeaderDragHandler>().Manager = manager;
            foreach (var edge in GetComponentsInChildren<ResizingEdge>())
            {
                edge.Manager = manager;
            }
        }

        #region Unity events

        protected void Awake()
        {
            TitleLabel = transform.Find("Header/Title").GetComponent<TMP_Text>();
            _edges = GetComponentsInChildren<ResizingEdge>();
            _header = transform.Find("Header").GetComponent<Image>();
            _content = transform.Find("Content").gameObject;
            if (!SavingSettings) return;
            _header.GetComponent<HeaderDragHandler>().OnDragged += _ => SaveSettings();
            foreach (var edge in _edges)
            {
                edge.OnResized += _ => SaveSettings();
            }
        }

        private void Start()
        {
            if (!SavingSettings) return;
            _windowSettings = new SettingsHistory<WindowSettingsBag>($"{name}.json", 1);
            if (_windowSettings.Recent.FirstOrDefault() is WindowSettingsBag bag)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(bag.X, bag.Y);
                GetComponent<RectTransform>().sizeDelta = new Vector2(bag.Width, bag.Height);
                if (bag.IsShowing) Show();
                else gameObject.SetActive(false);
                if (bag.IsMaximized)
                {
                    _maximizedHeight = bag.Height;
                    MinimizeButton.InitState(0);
                }
                else MinimizeButton.InitState(1);
            }

            _isInited = true;

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            var rects = _content.GetComponentsInChildren<ScrollRect>();
            foreach (var rect in rects)
            {
                rect.scrollSensitivity = 300;
            }
#endif
        }

        private void Update()
        {
            if (_edges.FirstOrDefault(e => e.IsHovered) is {} edge) edge.ShowEdgeCursor();
            else ResizingEdge.ShowDefaultCursor();
        }

        #endregion

        #region IPointDownHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
        }

        #endregion

        #region Private

        private ResizingEdge[] _edges;
        private Image _header;
        private GameObject _content;
        private float _maximizedHeight;
        private bool _isInited = false;
        private SettingsHistory<WindowSettingsBag> _windowSettings;

        private void SaveSettings()
        {
            if (!SavingSettings || !_isInited) return;
            var rect = GetComponent<RectTransform>();
            var bag = new WindowSettingsBag
            {
                X = rect.anchoredPosition.x,
                Y = rect.anchoredPosition.y,
                Width = rect.sizeDelta.x,
                Height = IsMinimized ? _maximizedHeight : rect.sizeDelta.y,
                IsMaximized = !IsMinimized,
                IsShowing = gameObject.activeSelf,
            };
            _windowSettings.Add(bag);
            _windowSettings.Save();
        }

        private IEnumerator HighlightHeader()
        {
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                _header.color = Color.Lerp(HighlightHeaderColor, BaseHeaderColor, i);
                yield return null;
            }
        }

        #endregion
    }
}