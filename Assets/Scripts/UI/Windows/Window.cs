using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<RectTransform> Collisions => _edges.Select(e => e.CollidedWindow).Where(w => w != null);

        public void Show()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(HighlightHeader());
                transform.SetAsLastSibling();
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        #region Unity events

        protected void Start()
        {
            _header = transform.Find("Header").GetComponent<Image>();
            _titleLabel = transform.Find("Header/Title").GetComponent<TMP_Text>();
            _edges = GetComponentsInChildren<ResizingEdge>();
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

        private Image _header;
        [SerializeField] private Color BaseHeaderColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color HighlightHeaderColor = Color.blue;
        private TMP_Text _titleLabel;
        private ResizingEdge[] _edges;

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