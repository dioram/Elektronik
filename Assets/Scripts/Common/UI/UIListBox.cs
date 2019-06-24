using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Elektronik.Common.UI
{
    public class UIListBox : MonoBehaviour
    {
        public class SelectionChangedEventArgs : EventArgs
        {
            public readonly int index;
            public SelectionChangedEventArgs(int idx)
            {
                index = idx;
            }
        }
        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
        public event SelectionChangedEventHandler OnSelectionChanged;

        public UIListBoxItem itemPrefab;
        private ObjectPool m_poolOfItems;

        private ScrollRect m_scrollView;
        private List<UIListBoxItem> m_listOfItems;

        private void Awake()
        {
            m_listOfItems = new List<UIListBoxItem>();
            m_scrollView = GetComponentInChildren<ScrollRect>();
            m_poolOfItems = new ObjectPool(itemPrefab.gameObject);
        }

        public UIListBoxItem this[int idx] { get { return m_listOfItems[idx]; } }

        public UIListBoxItem Add()
        {
            UIListBoxItem listViewItem = m_poolOfItems.Spawn().GetComponent<UIListBoxItem>();
            listViewItem.OnClick += SelectionChanged;
            listViewItem.transform.SetParent(m_scrollView.content);
            m_listOfItems.Add(listViewItem);
            return listViewItem;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender is UIListBoxItem, $"Sender must be {typeof(UIListBoxItem)}, but found {sender.GetType()}");
            int index = m_listOfItems.FindIndex(current => current == (UIListBoxItem)sender);
            Debug.Assert(index != -1, "UIListView wasn't found");
            SelectionChangedEventArgs selectionChangedEventArgs = new SelectionChangedEventArgs(index);
            OnSelectionChanged?.Invoke(this, selectionChangedEventArgs);
        }

        public void Remove(int idx)
        {
            UIListBoxItem listViewItem = m_listOfItems[idx];
            m_listOfItems.RemoveAt(idx);
            listViewItem.transform.SetParent(null);
            m_poolOfItems.Despawn(listViewItem.gameObject);
        }

        public void Remove(GameObject listViewItem)
        {
            int idx = m_listOfItems.FindIndex(obj => obj == listViewItem);
            Remove(idx);
        }

        public void Clear()
        {
            foreach (var item in m_listOfItems)
            {
                item.transform.SetParent(null);
                m_poolOfItems.Despawn(item.gameObject);
            }
            m_listOfItems.Clear();
        }
    }
}