using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private ScrollRect m_scrollView;
        private List<UIListBoxItem> m_listOfItems;

        private void Awake()
        {
            m_listOfItems = new List<UIListBoxItem>();
            m_scrollView = GetComponentInChildren<ScrollRect>();
        }

        public UIListBoxItem this[int idx] { get { return m_listOfItems[idx]; } }

        public void AddRange(IEnumerable<UIListBoxItem> objects)
        {
            foreach (var obj in objects)
            {
                Add(obj);
            }
        }

        public void Add(UIListBoxItem item)
        {
            UIListBoxItem listViewItem = MF_AutoPool.Spawn(item.gameObject).GetComponent<UIListBoxItem>();
            listViewItem.OnClick += SelectionChanged;
            listViewItem.transform.SetParent(m_scrollView.content);
            m_listOfItems.Add(listViewItem);
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            Debug.AssertFormat(sender is UIListBoxItem, "Sender must be {0}, but found {1}", typeof(UIListBoxItem), sender.GetType());
            int index = m_listOfItems.FindIndex(current => current == (UIListBoxItem)sender);
            Debug.Assert(index != -1, "UIListView wasn't found");
            SelectionChangedEventArgs selectionChangedEventArgs = new SelectionChangedEventArgs(index);
            if (OnSelectionChanged != null) OnSelectionChanged(this, selectionChangedEventArgs);
        }

        public void Remove(int idx)
        {
            UIListBoxItem listViewItem = m_listOfItems[idx];
            m_listOfItems.RemoveAt(idx);
            listViewItem.transform.SetParent(null);
            MF_AutoPool.Despawn(listViewItem.gameObject);
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
                MF_AutoPool.Despawn(item.gameObject);
            }
            m_listOfItems.Clear();
        }
    }
}