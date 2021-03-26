using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elektronik.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class ResizingEdge : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public enum EdgeSide
        {
            Top,
            Right,
            Bottom,
            Left,
        }

        public EdgeSide Edge;

        #region Unity event functions

        private void Start()
        {
            _resizeTarget = (RectTransform) transform.parent;
            var window = _resizeTarget.GetComponent<WindowBase>();
            _minHeigth = window.MinHeight;
            _minWidth = window.MinWidth;
            RectTransform testCanvas = _resizeTarget;
            while (_canvas == null && testCanvas != null)
            {
                _canvas = testCanvas.GetComponent<Canvas>();
                testCanvas = testCanvas.parent as RectTransform;
            }
        }

        private void Update()
        {
            if (!_hovered) return;
            switch (Edge)
            {
            case EdgeSide.Bottom:
            case EdgeSide.Top:
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.DoublePointedArrowPointingNorthAndSouth));
                break;
            case EdgeSide.Right:
            case EdgeSide.Left:
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.DoublePointedArrowPointingWestAndEast));
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region IDragHandler

        public void OnDrag(PointerEventData eventData)
        {
            var newPos = _resizeTarget.anchoredPosition;
            var newHeight = _resizeTarget.rect.height;
            var newWidth = _resizeTarget.rect.width;
            switch (Edge)
            {
            case EdgeSide.Top:
                newPos.y += eventData.delta.y / _canvas.scaleFactor;
                newHeight += eventData.delta.y / _canvas.scaleFactor;
                break;
            case EdgeSide.Right:
                newWidth += eventData.delta.x / _canvas.scaleFactor;
                break;
            case EdgeSide.Bottom:
                newHeight -= eventData.delta.y / _canvas.scaleFactor;
                break;
            case EdgeSide.Left:
                newPos.x += eventData.delta.x / _canvas.scaleFactor;
                newWidth -= eventData.delta.x / _canvas.scaleFactor;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }

            if (newHeight < _minHeigth || newWidth < _minWidth) return;
            _resizeTarget.anchoredPosition = newPos;
            _resizeTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            _resizeTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }

        #endregion

        #region IPointerEnterHandler

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
        }

        #endregion

        #region IPointerExitHandler

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
        }

        #endregion

        #region Private

        private RectTransform _resizeTarget;
        private Canvas _canvas;
        private float _minHeigth;
        private float _minWidth;
        private bool _hovered;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        private enum WindowsCursors
        {
            StandardArrowAndSmallHourglass = 32650,
            StandardArrow = 32512,
            Crosshair = 32515,
            Hand = 32649,
            ArrowAndQuestionMark = 32651,
            IBeam = 32513,
            SlashedCircle = 32648,
            FourPointedArrowPointingNorthSouthEastAndWest = 32646,
            DoublePointedArrowPointingNortheastAndSouthwest = 32643,
            DoublePointedArrowPointingNorthAndSouth = 32645,
            DoublePointedArrowPointingNorthwestAndSoutheast = 32642,
            DoublePointedArrowPointingWestAndEast = 32644,
            VerticalArrow = 32516,
            Hourglass = 32514
        }

        #endregion
    }
}