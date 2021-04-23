﻿using System;
using System.Collections;
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
            TopLeft = Top | Left,
            TopRight = Top | Right,
            BottomLeft = Bottom | Left,
            BottomRight = Bottom | Right,
        }

        public EdgeSide Side;
        public RectTransform ResizeTarget;
        public WindowsManager Manager;
        public Action<Rect> OnResized;

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

            StartCoroutine(ReturnToScreenRect());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (!_hovered) return;

            switch (Side)
            {
            case EdgeSide.Top:
            case EdgeSide.Bottom:
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.EdgeNorthAndSouth));
                break;
            case EdgeSide.Right:
            case EdgeSide.Left:
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.EdgeWestAndEast));
                break;
            case EdgeSide.TopLeft:
            case EdgeSide.BottomRight:
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.EdgeNorthwestAndSoutheast));
                break;
            case EdgeSide.TopRight:
            case EdgeSide.BottomLeft:
                SetCursor(LoadCursor(IntPtr.Zero, (int) WindowsCursors.EdgeNortheastAndSouthwest));
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
                    Manager.ShowLine(WindowsManager.Direction.Horizontal, topAligns.First());
                    StartCoroutine(Manager.HideAlignLine(WindowsManager.Direction.Horizontal));
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
                    Manager.ShowLine(WindowsManager.Direction.Horizontal, bottomAligns.First());
                    StartCoroutine(Manager.HideAlignLine(WindowsManager.Direction.Horizontal));
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
                    Manager.ShowLine(WindowsManager.Direction.Vertical, leftAligns.First());
                    StartCoroutine(Manager.HideAlignLine(WindowsManager.Direction.Vertical));
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
                    newWidth = rightAligns.First() - ResizeTarget.anchoredPosition.x;
                    Manager.ShowLine(WindowsManager.Direction.Vertical, rightAligns.First());
                    StartCoroutine(Manager.HideAlignLine(WindowsManager.Direction.Vertical));
                }
                else
                {
                    newWidth += eventData.delta.x / _canvas.scaleFactor;
                }
            }

            if (newHeight < _minHeight || newWidth < _minWidth) return;
            ResizeTarget.anchoredPosition = newPos;
            ResizeTarget.sizeDelta = new Vector2(newWidth, newHeight);
            OnResized?.Invoke(new Rect(ResizeTarget.anchoredPosition, ResizeTarget.sizeDelta));
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
            EdgeNortheastAndSouthwest = 32643,
            EdgeNorthAndSouth = 32645,
            EdgeNorthwestAndSoutheast = 32642,
            EdgeWestAndEast = 32644,
        }

        private IEnumerator ReturnToScreenRect()
        {
            while (true)
            {
                var pos = ResizeTarget.anchoredPosition;
                var size = ResizeTarget.sizeDelta;

                if (pos.x < 0)
                {
                    pos.x = 0;
                }
                else if (pos.x + size.x > Screen.width)
                {
                    pos.x = Screen.width - size.x;
                }

                if (pos.y > 0)
                {
                    pos.y = 0;
                }
                else if (pos.y - size.y < -Screen.height)
                {
                    pos.y = size.y - Screen.height;
                }

                ResizeTarget.anchoredPosition = pos;

                yield return new WaitForSeconds(2);
            }
        }

        #endregion
    }
}