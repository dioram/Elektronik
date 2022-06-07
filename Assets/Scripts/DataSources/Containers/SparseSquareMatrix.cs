using System;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Sparse matrix. </summary>
    /// <typeparam name="TElemType"> Type of matrix cells. </typeparam>
    internal class SparseSquareMatrix<TElemType> where TElemType : struct
    {
        /// <summary> Creates deep copy of this matrix. </summary>
        /// <returns> New matrix with same data. </returns>
        public SparseSquareMatrix<TElemType> DeepCopy()
        {
            var res = new SparseSquareMatrix<TElemType>();
            foreach (var row in _table)
            {
                if (!res._table.ContainsKey(row.Key)) res._table[row.Key] = new SortedDictionary<int, TElemType>();
                foreach (var col in row.Value)
                {
                    res._table[row.Key][col.Key] = col.Value;
                }
            }

            return res;
        }

        /// <summary> Checks if matrix have not null value at given row and column. </summary>
        /// <param name="row"> Row index. </param>
        /// <param name="col"> Column index. </param>
        /// <returns> true if there is not-null value, false otherwise. </returns>
        public bool Contains(int row, int col) => _table.ContainsKey(row) && _table[row].ContainsKey(col);

        public void Remove(int row, int col)
        {
            if (_table.ContainsKey(row))
            {
                _table[row].Remove(col);
            }
        }

        /// <summary> Returns indexes of columns that have not null values at given row. </summary>
        /// <param name="row"> Row index. </param>
        /// <returns></returns>
        public IList<int> GetColIndices(int row)
        {
            if (_table.ContainsKey(row)) return _table[row].Keys.ToList();
            return Array.Empty<int>();
        }

        /// <summary> Removes given row. </summary>
        /// <param name="row"> Row index. </param>
        public void RemoveRow(int row) => _table.Remove(row);

        public TElemType? this[int row, int col]
        {
            get
            {
                if (Contains(row, col)) return _table[row][col];
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

        /// <summary> Clears matrix. </summary>
        public void Clear() => _table.Clear();

        #region Private

        private readonly IDictionary<int, IDictionary<int, TElemType>> _table = new SortedDictionary<int, IDictionary<int, TElemType>>();
        
        private void Set(int row, int col, TElemType value)
        {
            if (!_table.ContainsKey(row))
                _table[row] = new SortedDictionary<int, TElemType>();
            _table[row][col] = value;
        }

        #endregion
    }
}