using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.UI.Localization;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Elektronik.UI.Windows
{
    public class WindowsManager : MonoBehaviour
    {
        [Range(0f, 10f)] public float AlignDistance;
        public RectTransform Canvas;
        public GameObject[] RendererWindowsPrefabs;
        public UILineRenderer HorizontalRenderer;
        public UILineRenderer VerticalRenderer;

        public List<Window> Windows = new List<Window>();

        public enum Direction
        {
            Vertical,
            Horizontal,
        }

        public void CreateWindow<TComponent>(string title, Action<TComponent, Window> callback,
                                             IList<object> titleFormatArgs = null)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                var prefab = RendererWindowsPrefabs.First(g => g.GetComponent<TComponent>() != null);
                var go = Instantiate(prefab, Canvas);
                var window = go.GetComponent<Window>();
                window.TitleLabel.SetLocalizedText(title, titleFormatArgs);
                SetManager(window);
                go.SetActive(false);
                Windows.Add(window);
                callback(go.GetComponent<TComponent>(), window);
            });
        }

        public void OnWindowDestroyed(Window sender)
        {
            Windows.Remove(sender);
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
                ShowLine(Direction.Horizontal, topAligns.First());
                StartCoroutine(HideAlignLine(Direction.Horizontal));
            }
            else if (bottomAligns.Length > 0)
            {
                pos.y = bottomAligns.First() + tr.sizeDelta.y;
                ShowLine(Direction.Horizontal, bottomAligns.First());
                StartCoroutine(HideAlignLine(Direction.Horizontal));
            }
            else
            {
                HorizontalRenderer.enabled = false;
            }

            if (leftAligns.Length > 0)
            {
                pos.x = leftAligns.First();
                ShowLine(Direction.Vertical, leftAligns.First());
                StartCoroutine(HideAlignLine(Direction.Vertical));
            }
            else if (rightAligns.Length > 0)
            {
                pos.x = rightAligns.First() - tr.sizeDelta.x;
                ShowLine(Direction.Vertical, rightAligns.First());
                StartCoroutine(HideAlignLine(Direction.Vertical));
            }
            else
            {
                VerticalRenderer.enabled = false;
            }

            tr.anchoredPosition = pos;
        }

        public void ShowLine(Direction direction, float value)
        {
            switch (direction)
            {
            case Direction.Vertical when VerticalRenderer != null:
                VerticalRenderer.enabled = true;
                VerticalRenderer.Points = new[] {new Vector2(value, 0), new Vector2(value, Screen.height)};
                VerticalRenderer.SetAllDirty();
                break;
            case Direction.Horizontal when HorizontalRenderer != null:
                HorizontalRenderer.enabled = true;
                HorizontalRenderer.Points = new[]
                {
                    new Vector2(0, Screen.height + value),
                    new Vector2(Screen.width, Screen.height + value)
                };
                HorizontalRenderer.SetAllDirty();
                break;
            }
        }

        public float[] NearestAlign(Direction direction, float value)
        {
            var arr = direction == Direction.Vertical ? VerticalAligns() : HorizontalAligns();
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return arr.Where(f => f != value)
                    .Where(f => Mathf.Abs(value - f) < AlignDistance)
                    .ToArray();
        }

        public IEnumerator HideAlignLine(Direction direction)
        {
            yield return new WaitForSeconds(1);
            switch (direction)
            {
            case Direction.Vertical when VerticalRenderer != null:
                VerticalRenderer.enabled = false;
                break;
            case Direction.Horizontal when HorizontalRenderer != null:
                HorizontalRenderer.enabled = false;
                break;
            }
        }

        #region Unity events

        private void Start()
        {
            foreach (var window in Windows)
            {
                SetManager(window);
            }
        }

        #endregion

        #region Private

        private void SetManager(Window window)
        {
            window.transform.Find("Header").GetComponent<HeaderDragHandler>().Manager = this;
            foreach (var edge in window.GetComponentsInChildren<ResizingEdge>())
            {
                edge.Manager = this;
            }
        }

        private float[] VerticalAligns()
        {
            return Windows
                    .Where(w => w.gameObject.activeInHierarchy && !w.IsMinimized)
                    .Select(w => (RectTransform) w.transform)
                    .SelectMany(t => new[] {t.anchoredPosition.x, t.anchoredPosition.x + t.sizeDelta.x})
                    .Concat(new[] {0, (float) Screen.width})
                    .ToArray();
        }

        private float[] HorizontalAligns()
        {
            return Windows
                    .Where(w => w.gameObject.activeInHierarchy && !w.IsMinimized)
                    .Select(w => (RectTransform) w.transform)
                    .SelectMany(t => new[] {t.anchoredPosition.y - t.sizeDelta.y, t.anchoredPosition.y})
                    .Concat(new[] {0, -50, -Screen.height, (float) 40 - Screen.height})
                    .ToArray();
        }

        #endregion
    }
}