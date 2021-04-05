using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elektronik.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class ResizingEdge : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Flags]
        public enum EdgeSide
        {
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8,
        }

        public EdgeSide Edge;
        public RectTransform ResizeTarget;
        [CanBeNull] public RectTransform CollidedWindow;

        #region Unity event functions

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
            var window = ResizeTarget.GetComponent<Window>();
            _minHeight = window.MinHeight;
            _minWidth = window.MinWidth;
            RectTransform testCanvas = ResizeTarget;
            while (_canvas == null && testCanvas != null)
            {
                _canvas = testCanvas.GetComponent<Canvas>();
                testCanvas = testCanvas.parent as RectTransform;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CollidedWindow = (RectTransform) other.transform;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CollidedWindow = null;
        }

        private void Update()
        {
            if (!_hovered) return;
            if (((Edge & (EdgeSide.Left | EdgeSide.Top)) == (EdgeSide.Left | EdgeSide.Top))
                || ((Edge & (EdgeSide.Right | EdgeSide.Bottom)) == (EdgeSide.Right | EdgeSide.Bottom)))
            {
                SetCursor(LoadCursor(IntPtr.Zero,
                                     (int) WindowsCursors.DoublePointedArrowPointingNorthwestAndSoutheast));
            }
            else if (((Edge & (EdgeSide.Left | EdgeSide.Bottom)) == (EdgeSide.Left | EdgeSide.Bottom))
                || ((Edge & (EdgeSide.Right | EdgeSide.Top)) == (EdgeSide.Right | EdgeSide.Top)))
            {
                SetCursor(LoadCursor(IntPtr.Zero,
                                     (int) WindowsCursors.DoublePointedArrowPointingNortheastAndSouthwest));
            }
            else if (((Edge & EdgeSide.Top) == EdgeSide.Top)
                || ((Edge & EdgeSide.Bottom) == EdgeSide.Bottom))
            {
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.DoublePointedArrowPointingNorthAndSouth));
            }
            else if (((Edge & EdgeSide.Left) == EdgeSide.Left)
                || ((Edge & EdgeSide.Right) == EdgeSide.Right))
            {
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.DoublePointedArrowPointingWestAndEast));
            }
        }

        #endregion

        #region IDragHandler

        public void OnDrag(PointerEventData eventData)
        {
            var newPos = ResizeTarget.anchoredPosition;
            var newHeight = ResizeTarget.rect.height;
            var newWidth = ResizeTarget.rect.width;
            if ((Edge & EdgeSide.Top) == EdgeSide.Top)
            {
                newPos.y += eventData.delta.y / _canvas.scaleFactor;
                newHeight += eventData.delta.y / _canvas.scaleFactor;
            }

            if ((Edge & EdgeSide.Bottom) == EdgeSide.Bottom)
            {
                newHeight -= eventData.delta.y / _canvas.scaleFactor;
            }

            if ((Edge & EdgeSide.Left) == EdgeSide.Left)
            {
                newPos.x += eventData.delta.x / _canvas.scaleFactor;
                newWidth -= eventData.delta.x / _canvas.scaleFactor;
            }

            if ((Edge & EdgeSide.Right) == EdgeSide.Right)
            {
                newWidth += eventData.delta.x / _canvas.scaleFactor;
            }

            if (newHeight < _minHeight || newWidth < _minWidth) return;
            ResizeTarget.anchoredPosition = newPos;
            ResizeTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            ResizeTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
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

        private BoxCollider2D _collider;
        private Canvas _canvas;
        private float _minHeight;
        private float _minWidth;
        private bool _hovered;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        private enum WindowsCursors
        {
            StandardArrow = 32512,
            DoublePointedArrowPointingNortheastAndSouthwest = 32643,
            DoublePointedArrowPointingNorthAndSouth = 32645,
            DoublePointedArrowPointingNorthwestAndSoutheast = 32642,
            DoublePointedArrowPointingWestAndEast = 32644,
        }

        #endregion
    }
}