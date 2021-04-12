using UnityEngine;
using UnityEngine.EventSystems;

namespace Elektronik.UI.Windows
{
    public class HeaderDragHandler : MonoBehaviour, IDragHandler
    {
        public WindowsManager Manager;
        
        #region Unity events

        private void Start()
        {
            _windowTransform = transform.parent as RectTransform;
            RectTransform testCanvas = _windowTransform;
            while (_canvas == null && testCanvas != null)
            {
                _canvas = testCanvas.GetComponent<Canvas>();
                testCanvas = testCanvas.parent as RectTransform;
            }
        }

        private void OnDestroy()
        {
            Manager.OnWindowDestroyed(_windowTransform.GetComponent<Window>());
        }

        #endregion

        #region IDragHandler

        public void OnDrag(PointerEventData eventData)
        {
            var oldPos = _windowTransform.anchoredPosition;
            var newPos = oldPos;

            newPos += eventData.delta / _canvas.scaleFactor;
            _windowTransform.anchoredPosition = newPos;
            Manager.Align(_windowTransform);
        }

        #endregion

        #region Private

        private RectTransform _windowTransform;
        private Canvas _canvas;

        #endregion
    }
}