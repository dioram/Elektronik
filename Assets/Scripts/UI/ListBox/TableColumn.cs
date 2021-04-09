using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.UI.ListBox
{
    public class TableColumn : ListBoxItem, IPointerEnterHandler, IPointerExitHandler
    {
        [Range(0, 50)] public int MaxAmountOfRows;

        public RectTransform Window;

        public void LoadData(string[] data)
        {
            Clear();
            MaxAmountOfRows = data.Length;
            foreach (var s in data)
            {
                _rows.Enqueue(s);
            }

            DataLabel.text = string.Join("\n", _rows.ToArray());
        }

        public void SetHeader(string columnName)
        {
            _headerLabel.text = columnName;
        }

        public void AddRow(string data)
        {
            if (_rows.Count == MaxAmountOfRows) _rows.Dequeue();
            _rows.Enqueue(data);
            DataLabel.text = string.Join("\n", _rows.ToArray());
            UpdateSize();
        }

        public void Clear()
        {
            _rows.Clear();
            DataLabel.text = "";
        }

        #region Unity events

        protected override void Awake()
        {
            _rect = (RectTransform) transform;
            _headerLabel = HeaderButton.GetComponentInChildren<TMP_Text>();
            HeaderButton.OnClickAsObservable()
                    .Select(_ => _isVisible = !_isVisible)
                    .Do(b => _headerLabel.enabled = b)
                    .Do(b => HeaderIcon.gameObject.SetActive(b))
                    .Do(b => DataLabel.gameObject.SetActive(b))
                    .Subscribe(_ => UpdateSize());
        }

        protected override void Start()
        {
            _allColumns = transform.parent
                    .GetComponentsInChildren<TableColumn>()
                    .OrderBy(t => t.transform.GetSiblingIndex())
                    .ToArray();
            UpdateSize();
        }

        private void Update()
        {
            // Check if this is last visible column
            bool isLast = _allColumns.LastOrDefault(t => t._isVisible) == this;
            // if this became not last visible column - return to standard size
            if (_isLast != isLast && isLast == false)
            {
                UpdateSize();
            }

            _isLast = isLast;

            if (!_isLast) return;

            float scrollBarDelta = 0;
            // ReSharper disable once Unity.NoNullPropagation
            var scrollBar = transform.parent
                    .GetComponentInParent<ScrollRect>()?
                    .verticalScrollbar?
                    .transform as RectTransform;
            if (scrollBar != null && scrollBar.gameObject.activeInHierarchy) scrollBarDelta = scrollBar.sizeDelta.x;
            var invisibleDelta = (transform.parent.childCount - transform.GetSiblingIndex()) * HiddenWidth;
            var delta = (_rect.anchoredPosition.x + _preferredWidth)
                    - (Window.sizeDelta.x - invisibleDelta - scrollBarDelta);
            if (delta < 0)
            {
                _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _preferredWidth - delta);
            }
        }

        private void OnGUI()
        {
            if (_showTooltip)
            {
                GUI.Box(new Rect(Window.position.x + 5, Screen.height - (Window.position.y + 30), 300, 25),
                        _headerLabel.text);
            }
        }

        #endregion

        #region IPoint*Handler

        public void OnPointerEnter(PointerEventData eventData)
        {
            _showTooltip = !_isVisible;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _showTooltip = false;
        }

        #endregion

        #region Private

        [SerializeField] private TMP_Text DataLabel;
        [SerializeField] private Button HeaderButton;
        private TMP_Text _headerLabel;
        [SerializeField] private Image HeaderIcon;
        [SerializeField, Range(0, 50)] private float HiddenWidth = 10;
        private TableColumn[] _allColumns;
        private readonly Queue<string> _rows = new Queue<string>(50);
        private bool _isVisible = true;
        private RectTransform _rect;
        private float _preferredWidth;
        private bool _isLast;
        private bool _showTooltip = false;

        private void UpdateSize()
        {
            var dataSize = new Vector2(DataLabel.preferredWidth, DataLabel.preferredHeight);
            _preferredWidth = Mathf.Max(_headerLabel.renderedWidth + 30, dataSize.x) + 10;
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dataSize.y + 35);

            if (_isVisible)
            {
                _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _preferredWidth);
            }
            else
            {
                _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HiddenWidth);
            }
        }

        #endregion
    }
}