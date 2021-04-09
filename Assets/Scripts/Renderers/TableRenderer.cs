using System.Collections.Generic;
using System.Linq;
using Elektronik.UI.ListBox;
using UnityEngine;

namespace Elektronik.Renderers
{
    public class TableRenderer : MonoBehaviour, IDataRenderer<string[]>
    {
        [Range(0, 50)]
        public int MaxAmountOfRows;

        public void LoadData(string[] header, string[][] data)
        {
            Clear();
            SetHeader(header);
            for (int i = 0; i < data.Length; i++)
            {
                _columns[i].LoadData(data[i]);
            }
        }
        
        public void SetHeader(string[] header)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                Table.Clear();
                var rect = (RectTransform) transform;
                foreach (var columnName in header)
                {
                    var column = Table.Add().GetComponent<TableColumn>();
                    column.MaxAmountOfRows = MaxAmountOfRows;
                    column.SetHeader(columnName);
                    column.Window = rect;
                    _columns.Add(column);
                }

                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x + 1);
            });
        }

        #region Unity events

        private void Start()
        {
            if (Table == null) Table = GetComponentInChildren<ListBox>();
        }

        private void OnEnable()
        {
            IsShowing = true;
        }

        private void OnDisable()
        {
            IsShowing = false;
        }

        #endregion

        #region IDataRenderer

        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                if (_isShowing == value) return;
                _isShowing = value;
                MainThreadInvoker.Instance.Enqueue(() => gameObject.SetActive(_isShowing));
            }
        }

        public void Render(string[] data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                foreach (var pair in data.Zip(_columns, (s, column) => (s, column)))
                {
                    pair.column.AddRow(pair.s);
                }
            });
        }

        public void Clear()
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                foreach (var column in _columns)
                {
                    column.Clear();
                }
            });
        }

        #endregion

        #region Private

        [SerializeField] private ListBox Table;
        private readonly List<TableColumn> _columns = new List<TableColumn>();
        private bool _isShowing;

        #endregion
    }
}