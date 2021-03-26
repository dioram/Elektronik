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
            _window = transform.parent as RectTransform;
            RectTransform testCanvas = _window;
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
            var oldPos = _window.anchoredPosition;
            _window.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            if (!IsInsideScreen()) _window.anchoredPosition = oldPos;
        }

        #endregion

        #region Private

        private RectTransform _window;
        private Canvas _canvas;
        
        private bool IsInsideScreen()
        {
            var corners = new Vector3[4];
            _window.GetWorldCorners(corners);
            var rect = new Rect(0, 0, Screen.width, Screen.height);
            return corners.All(corner => rect.Contains(corner));
        }

        #endregion
    }
}