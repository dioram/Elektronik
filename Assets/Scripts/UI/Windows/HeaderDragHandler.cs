using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elektronik.UI.Windows
{
    public class HeaderDragHandler : MonoBehaviour, IDragHandler
    {
        #region Unity events
        
        private void Start()
        {
            _window = transform.parent.GetComponent<Window>();
            _windowTransform = transform.parent as RectTransform;
            RectTransform testCanvas = _windowTransform;
            while (_canvas == null && testCanvas != null)
            {
                _canvas = testCanvas.GetComponent<Canvas>();
                testCanvas = testCanvas.parent as RectTransform;
            }
        }

        #endregion

        #region IDragHandler
        
        public void OnDrag(PointerEventData eventData)
        {
            var oldPos = _windowTransform.anchoredPosition;
            _windowTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            if (!IsInsideScreen()) _windowTransform.anchoredPosition = oldPos;
        }

        #endregion

        #region Private

        private Window _window;
        private RectTransform _windowTransform;
        private Canvas _canvas;
        
        private bool IsInsideScreen()
        {
            var corners = new Vector3[4];
            _windowTransform.GetWorldCorners(corners);
            var rect = new Rect(0, 0, Screen.width, Screen.height);
            return corners.All(corner => rect.Contains(corner));
        }

        #endregion
    }
}