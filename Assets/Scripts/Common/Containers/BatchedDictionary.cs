using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.Containers
{
    public class BatchedDictionary<TValue> : IDictionary<int, TValue>
    {
        private int m_batchSize;
        private List<int[]> m_keys;
        private List<TValue[]> m_values;
        private Dictionary<int, int> m_batchNum2offset;
        private Dictionary<int, int> m_batchCount;

        public BatchedDictionary(int blockSize = 65000)
        {
            m_batchSize = blockSize;
            m_batchCount = new Dictionary<int, int>();
            m_batchNum2offset = new Dictionary<int, int>();
            m_keys = new List<int[]>();
            m_values = new List<TValue[]>();
            Count = 0;
        }

        public TValue this[int key] 
        {
            get 
            {
                int batchNum = key / m_batchSize;
                int offset = m_batchNum2offset[batchNum];
                int keyOffset = key % m_batchSize;
                if (m_keys[offset][keyOffset] != -1)
                    return m_values[offset][keyOffset];
                throw new KeyNotFoundException();
            } 
            set => Add_(key, value); 
        }

        public ICollection<int> Keys => m_keys.SelectMany(k => k.Where(v => v != -1)).ToList();

        public ICollection<TValue> Values => 
            m_keys
            .SelectMany(k => k.Where(v => v != -1))
            .Select(k => this[k]).ToList();

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        private void Alloc(int key)
        {
            int batchNum = key / m_batchSize;
            if (!m_batchNum2offset.ContainsKey(batchNum))
            {
                int lastId = m_keys.Count;
                m_batchNum2offset[batchNum] = lastId;
                m_batchCount[lastId] = 0;
                m_keys.Add(new int[m_batchSize]);
                m_values.Add(new TValue[m_batchSize]);
                for (int i = 0; i < m_batchSize; ++i) m_keys[lastId][i] = -1;
            }
        }

        private void Add_(int key, TValue value)
        {
            Alloc(key);
            int batchNum = key / m_batchSize;
            int offset = m_batchNum2offset[batchNum];
            int keyOffset = key % m_batchSize;
            if (m_keys[offset][keyOffset] == -1)
            {
                m_batchCount[offset] += 1;
                Count += 1;
            }
            m_keys[offset][keyOffset] = key;
            m_values[offset][keyOffset] = value;
        }

        public void Add(int key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("An element with the same key already exists");
            Add_(key, value);
        }

        public void Add(KeyValuePair<int, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            Count = 0;
            m_batchCount.Clear();
            m_batchNum2offset.Clear();
            m_keys.Clear();
            m_values.Clear();
        }

        public bool Contains(KeyValuePair<int, TValue> item)
        {
            int batchNum = item.Key / m_batchSize;
            int batchOffset = m_batchNum2offset[batchNum];
            int keyOffset = item.Key % m_batchSize;
            return m_keys[batchOffset][keyOffset] != -1 && m_values[batchOffset][keyOffset].Equals(item.Value);
        }

        public bool ContainsKey(int key)
        {
            int batchNum = key / m_batchSize;
            if (m_batchNum2offset.TryGetValue(batchNum, out var offset))
            {
                int keyOffset = key % m_batchSize;
                return m_keys[offset][keyOffset] != -1;
            }
            return false;
        }

        public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
        {
            int idx = 0;
            for (int i = 0; i < m_keys.Count; ++i)
            {
                for (int j = 0; j < m_batchSize; ++j)
                {
                    if (m_keys[i][j] != -1)
                    {
                        array[arrayIndex + idx] = new KeyValuePair<int, TValue>(m_keys[i][j], m_values[i][j]);
                        ++idx;
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
            => m_keys
            .SelectMany(k => k.Where(v => v != -1))
            .Select(k => new KeyValuePair<int, TValue>(k, this[k]))
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(int key)
        {
            if (ContainsKey(key))
            {
                int batchNum = key / m_batchSize;
                int offset = m_batchNum2offset[batchNum];
                int keyOffset = key % m_batchSize;
                Count -= 1;
                m_batchCount[offset] -= 1;
                if (m_batchCount[offset] == 0)
                {
                    m_keys.RemoveAt(offset);
                    m_values.RemoveAt(offset);
                }
                else
                {
                    m_keys[offset][keyOffset] = -1;
                    m_values[offset][keyOffset] = default;
                }
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<int, TValue> item) => Remove(item.Key);

        public bool TryGetValue(int key, out TValue value)
        {
            value = default;
            int batchNum = key / m_batchSize;
            if (m_batchNum2offset.TryGetValue(batchNum, out var offset))
            {
                int keyOffset = key % m_batchSize;
                int key_ = m_keys[offset][keyOffset];
                if (key_ != -1)
                {
                    value = m_values[offset][keyOffset];
                    return true;
                }
            }
            return false;
        }
    }
}
