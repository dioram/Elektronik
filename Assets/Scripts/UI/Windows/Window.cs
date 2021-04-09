using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour, IPointerDownHandler
    {
        public float MinHeight = 40;
        public float MinWidth = 80;
        public string Title;
        public bool IsMinimized => _isMinimized;

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
        }

        public void Minimize()
        {
            _isMinimized = true;
            var rect = ((RectTransform) transform);
            _maximizedHeight = rect.sizeDelta.y;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 42);
            _content.SetActive(false);
            foreach (var edge in _edges)
            {
                edge.enabled = false;
            }
        }

        public void Maximize()
        {
            _isMinimized = false;
            var rect = ((RectTransform) transform);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, _maximizedHeight);
            _content.SetActive(true);
            foreach (var edge in _edges)
            {
                edge.enabled = true;
            }
        }

        #region Unity events

        protected void Start()
        {
            _edges = GetComponentsInChildren<ResizingEdge>();
            _header = transform.Find("Header").GetComponent<Image>();
            _content = transform.Find("Content").gameObject;
            _titleLabel = transform.Find("Header/Title").GetComponent<TMP_Text>();
            if (!string.IsNullOrEmpty(Title))
            {
                _titleLabel.text = Title;
            }
        }

        #endregion

        #region IPointDownHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
        }

        #endregion

        #region Private

        [SerializeField] private Color BaseHeaderColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color HighlightHeaderColor = Color.blue;
        private ResizingEdge[] _edges;
        private Image _header;
        private TMP_Text _titleLabel;
        private GameObject _content;
        private float _maximizedHeight;
        private bool _isMinimized;

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