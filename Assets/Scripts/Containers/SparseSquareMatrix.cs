using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Containers
{
    public class SparseSquareMatrix<TElemType> where TElemType : struct
    {
        private readonly IDictionary<int, IDictionary<int, TElemType>> _table;

        public SparseSquareMatrix<TElemType> DeepCopy()
        {
            var res = new SparseSquareMatrix<TElemType>();
            foreach (var row in _table)
            {
                if (!_table.ContainsKey(row.Key)) _table[row.Key] = new SortedDictionary<int, TElemType>();
                foreach (var col in row.Value)
                {
                    _table[row.Key][col.Key] = col.Value;
                }
            }

            return res;
        }

        public bool Contains(int row, int col)
        {
            return _table.ContainsKey(row) && _table[row].ContainsKey(col);
        }

        public void Remove(int row, int col)
        {
            if (_table.ContainsKey(row))
            {
                _table[row].Remove(col);
            }
        }

        public IEnumerable<TElemType> GetRow(int row)
        {
            if (_table.ContainsKey(row))
                return _table[row].Values.ToList();
            return Enumerable.Empty<TElemType>();
        }

        public IEnumerable<int> GetColIndices(int row)
        {
            if (_table.ContainsKey(row))
                return _table[row].Keys.ToList();
            return Enumerable.Empty<int>();
        }

        public void RemoveRow(int row) => _table.Remove(row);

        public SparseSquareMatrix()
        {
            _table = new SortedDictionary<int, IDictionary<int, TElemType>>();
        }

        private void Set(int row, int col, TElemType value)
        {
            if (!_table.ContainsKey(row))
                _table[row] = new SortedDictionary<int, TElemType>();
            _table[row][col] = value;
        }

        public TElemType? this[int row, int col]
        {
            get
            {
                if (_table.ContainsKey(row) && _table[row].ContainsKey(col))
                    return _table[row][col];
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    Set(row, col, value.Value);
                }
            }
        }

        public void Clear() => _table.Clear();
    }
}