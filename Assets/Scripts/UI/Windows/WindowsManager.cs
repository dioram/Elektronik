using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.UI.Windows
{
    public class WindowsManager : MonoBehaviour
    {
        [Range(0f, 10f)] public float AlignDistance;
        public RectTransform Canvas;
        public GameObject[] RendererWindowsPrefabs;

        public List<Window> Windows = new List<Window>();
        
        public enum Direction
        {
            Vertical,
            Horizontal,
        }

        public void CreateWindow<TComponent>(string title, Action<TComponent, Window> callback)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                var prefab = RendererWindowsPrefabs.First(g => g.GetComponent<TComponent>() != null);
                var go = Instantiate(prefab, Canvas);
                var window = go.GetComponent<Window>();
                window.Title = title;
                window.transform.Find("Header").GetComponent<HeaderDragHandler>().Manager = this;
                foreach (var edge in window.GetComponentsInChildren<ResizingEdge>())
                {
                    edge.Manager = this;
                }
                go.SetActive(false);
                Windows.Add(window);
                callback(go.GetComponent<TComponent>(), window);
            });
        }

        public void Align(RectTransform tr)
        {
            var pos = tr.anchoredPosition;
            var top = pos.y;
            var bottom = pos.y - tr.sizeDelta.y;
            var left = pos.x;
            var right = pos.x + tr.sizeDelta.x;

            var topAligns = NearestAlign(Direction.Horizontal, top);
            var bottomAligns = NearestAlign(Direction.Horizontal, bottom);
            var leftAligns = NearestAlign(Direction.Vertical, left);
            var rightAligns = NearestAlign(Direction.Vertical, right);
            if (topAligns.Length > 0)
            {
                pos.y = topAligns.First();
            }
            else if (bottomAligns.Length > 0)
            {
                pos.y = bottomAligns.First() + tr.sizeDelta.y;
            }

            if (leftAligns.Length > 0)
            {
                pos.x = leftAligns.First();
            }
            else if (rightAligns.Length > 0)
            {
                pos.x = rightAligns.First() - tr.sizeDelta.x;
            }

            tr.anchoredPosition = pos;
        }

        public void AlignResizing(RectTransform tr, ResizingEdge.EdgeSide side)
        {
            var pos = tr.anchoredPosition;
            var height = tr.rect.height;
            var width = tr.rect.width;
            if ((side & ResizingEdge.EdgeSide.Top) == ResizingEdge.EdgeSide.Top)
            {
                var top = tr.anchoredPosition.y;
                var topAligns = NearestAlign(Direction.Horizontal, top);
                if (topAligns.Length > 0)
                {
                    pos.y = topAligns.First();
                    height += pos.y - topAligns.First();
                }
            }
            else if ((side & ResizingEdge.EdgeSide.Bottom) == ResizingEdge.EdgeSide.Bottom)
            {
                var bottom = tr.anchoredPosition.y - tr.sizeDelta.y;
                var bottomAligns = NearestAlign(Direction.Horizontal, bottom);
                if (bottomAligns.Length > 0)
                {
                    height += bottom - bottomAligns.First();
                }
            }
            if ((side & ResizingEdge.EdgeSide.Left) == ResizingEdge.EdgeSide.Left)
            {
                var left = tr.anchoredPosition.x;
                var leftAligns = NearestAlign(Direction.Vertical, left);
                if (leftAligns.Length > 0)
                {
                    pos.x = leftAligns.First();
                    width += pos.x - leftAligns.First();
                }
            }
            else if ((side & ResizingEdge.EdgeSide.Right) == ResizingEdge.EdgeSide.Right)
            {
                var right = tr.anchoredPosition.x + tr.sizeDelta.x;
                var rightAligns = NearestAlign(Direction.Vertical, right);
                if (rightAligns.Length > 0)
                {
                    width += right - rightAligns.First();
                }
            }

            tr.anchoredPosition = pos;
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        public float[] NearestAlign(Direction direction, float value)
        {
            var arr = direction == Direction.Vertical ? VerticalAligns() : HorizontalAligns();
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return arr.Where(f => f != value)
                    .Where(f => Mathf.Abs(value - f) < AlignDistance)
                    .ToArray();
        }

        #region Private

        private float[] VerticalAligns()
        {
            return Windows
                    .Where(w => w.gameObject.activeInHierarchy)
                    .Select(w => (RectTransform) w.transform)
                    .SelectMany(t => new[] {t.anchoredPosition.x, t.anchoredPosition.x + t.sizeDelta.x})
                    .Concat(new[] {0, (float) Screen.width})
                    .ToArray();
        }

        private float[] HorizontalAligns()
        {
            return Windows
                    .Where(w => w.gameObject.activeInHierarchy)
                    .Select(w => (RectTransform) w.transform)
                    .SelectMany(t => new[] {t.anchoredPosition.y - t.sizeDelta.y, t.anchoredPosition.y})
                    .Concat(new[] {0, (float) -Screen.height})
                    .ToArray();
        }

        #endregion
    }
}