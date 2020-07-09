using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public class SparseSquareMatrix<ElemType> where ElemType : struct
    {
        private IDictionary<int, IDictionary<int, ElemType>> m_table;
        public void Remove(int row, int col)
        {
            if (m_table.ContainsKey(row))
            {
                m_table[row].Remove(col);
            }
        }

        public IEnumerable<ElemType> GetRow(int row)
        {
            if (m_table.ContainsKey(row))
                return m_table[row].Values;
            return Enumerable.Empty<ElemType>();
        }
        public IEnumerable<int> GetColIndices(int row)
        {
            if (m_table.ContainsKey(row))
                return m_table[row].Keys;
            return Enumerable.Empty<int>();
        }
        public void RemoveRow(int row) => m_table.Remove(row);

        public SparseSquareMatrix()
        {
            m_table = new SortedDictionary<int, IDictionary<int, ElemType>>();
        }

        private void Set(int row, int col, ElemType value)
        {
            if (!m_table.ContainsKey(row))
                m_table[row] = new SortedDictionary<int, ElemType>();
            m_table[row][col] = value;
        }

        public ElemType? this[int row, int col]
        {
            get
            {
                if (m_table.ContainsKey(row) && m_table[row].ContainsKey(col))
                    return m_table[row][col];
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

        public void Clear() => m_table.Clear();
    }
}
