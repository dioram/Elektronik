using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.ListBox
{
    public class TableColumn : ListBoxItem
    {
        [Range(0, 50)] public int MaxAmountOfRows;

        public RectTransform Window;

        public void SetHeader(string columnName)
        {
            HeaderLabel.text = columnName;
        }

        public void AddRow(string data)
        {
            if (_rows.Count == MaxAmountOfRows) _rows.Dequeue();
            _rows.Enqueue(data);
            if (DataLabel.enabled) DataLabel.text = string.Join("\n", _rows.ToArray());
        }

        public void Clear()
        {
            DataLabel.text = "";
        }

        #region Unity events

        private void Awake()
        {
            HeaderLabel = HeaderButton.GetComponentInChildren<TMP_Text>();
            HeaderButton.OnClickAsObservable()
                    .Select(_ => _isVisible = !_isVisible)
                    .Do(b => HeaderLabel.enabled = DataLabel.enabled = b)
                    .Do(b => Window.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                                                              Window.sizeDelta.x + (b ? -1 : 1)))
                    .Subscribe();
        }

        #endregion

        #region Private

        [SerializeField] private TMP_Text DataLabel;
        [SerializeField] private Button HeaderButton;
        [SerializeField] private TMP_Text HeaderLabel;
        private readonly Queue<string> _rows = new Queue<string>(50);
        private bool _isVisible = true;

        #endregion
    }
}