using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
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

        public EdgeSide Side;
        public RectTransform ResizeTarget;
        public WindowsManager Manager;

        #region Unity event functions

        private void Start()
        {
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

        private void Update()
        {
            if (!_hovered) return;
            if (((Side & (EdgeSide.Left | EdgeSide.Top)) == (EdgeSide.Left | EdgeSide.Top))
                || ((Side & (EdgeSide.Right | EdgeSide.Bottom)) == (EdgeSide.Right | EdgeSide.Bottom)))
            {
                SetCursor(LoadCursor(IntPtr.Zero,
                                     (int) WindowsCursors.DoublePointedArrowPointingNorthwestAndSoutheast));
            }
            else if (((Side & (EdgeSide.Left | EdgeSide.Bottom)) == (EdgeSide.Left | EdgeSide.Bottom))
                || ((Side & (EdgeSide.Right | EdgeSide.Top)) == (EdgeSide.Right | EdgeSide.Top)))
            {
                SetCursor(LoadCursor(IntPtr.Zero,
                                     (int) WindowsCursors.DoublePointedArrowPointingNortheastAndSouthwest));
            }
            else if (((Side & EdgeSide.Top) == EdgeSide.Top)
                || ((Side & EdgeSide.Bottom) == EdgeSide.Bottom))
            {
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.DoublePointedArrowPointingNorthAndSouth));
            }
            else if (((Side & EdgeSide.Left) == EdgeSide.Left)
                || ((Side & EdgeSide.Right) == EdgeSide.Right))
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
            if ((Side & EdgeSide.Top) == EdgeSide.Top)
            {
                var topAligns = Manager.NearestAlign(WindowsManager.Direction.Horizontal, newPos.y);
                if (topAligns.Length > 0)
                {
                    newHeight += topAligns.First() - newPos.y;
                    newPos.y = topAligns.First();
                }
                else
                {
                    newPos.y += eventData.delta.y / _canvas.scaleFactor;
                    newHeight += eventData.delta.y / _canvas.scaleFactor;
                }
            }

            if ((Side & EdgeSide.Bottom) == EdgeSide.Bottom)
            {
                var bottom = ResizeTarget.anchoredPosition.y - ResizeTarget.sizeDelta.y;
                var bottomAligns = Manager.NearestAlign(WindowsManager.Direction.Horizontal, bottom);
                if (bottomAligns.Length > 0)
                {
                    newHeight += bottom - bottomAligns.First();
                }
                else
                {
                    newHeight -= eventData.delta.y / _canvas.scaleFactor;
                }
            }

            if ((Side & EdgeSide.Left) == EdgeSide.Left)
            {
                var leftAligns = Manager.NearestAlign(WindowsManager.Direction.Vertical, newPos.x);
                if (leftAligns.Length > 0)
                {
                    newWidth -= leftAligns.First() - newPos.x;
                    newPos.x = leftAligns.First();
                }
                else
                {
                    newPos.x += eventData.delta.x / _canvas.scaleFactor;
                    newWidth -= eventData.delta.x / _canvas.scaleFactor;
                }
            }

            if ((Side & EdgeSide.Right) == EdgeSide.Right)
            {
                var right = ResizeTarget.anchoredPosition.x + ResizeTarget.sizeDelta.x;
                var rightAligns = Manager.NearestAlign(WindowsManager.Direction.Vertical, right);
                if (rightAligns.Length > 0)
                {
                    newWidth += right - rightAligns.First();
                }
                else
                {
                    newWidth += eventData.delta.x / _canvas.scaleFactor;
                }
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

        private Canvas _canvas;
        private float _minHeight;
        private float _minWidth;
        private bool _hovered;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
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