using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class ListBox : MonoBehaviour, IEnumerable<ListBoxItem>
    {
        public class SelectionChangedEventArgs : EventArgs
        {
            public readonly int Index;

            public SelectionChangedEventArgs(int idx)
            {
                Index = idx;
            }
        }

        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

        public event SelectionChangedEventHandler OnSelectionChanged;

        public ListBoxItem itemPrefab;
        private ObjectPool _poolOfItems;

        private ScrollRect _scrollView;
        private List<ListBoxItem> _listOfItems;

        private void Awake()
        {
            _listOfItems = new List<ListBoxItem>();
            _scrollView = GetComponentInChildren<ScrollRect>();
            _poolOfItems = new ObjectPool(itemPrefab.gameObject);
        }

        public ListBoxItem this[int idx] => _listOfItems[idx];

        public ListBoxItem Add()
        {
            ListBoxItem listViewItem = _poolOfItems.Spawn().GetComponent<ListBoxItem>();
            listViewItem.OnClick += SelectionChanged;
            listViewItem.transform.SetParent(_scrollView.content);
            _listOfItems.Add(listViewItem);
            return listViewItem;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender is ListBoxItem,
                         $"Sender must be {typeof(ListBoxItem)}, but found {sender.GetType()}");
            int index = _listOfItems.FindIndex(current => current == (ListBoxItem) sender);
            Debug.Assert(index != -1, "UIListView wasn't found");
            SelectionChangedEventArgs selectionChangedEventArgs = new SelectionChangedEventArgs(index);
            OnSelectionChanged?.Invoke(this, selectionChangedEventArgs);
        }

        public void Remove(int idx)
        {
            ListBoxItem listViewItem = _listOfItems[idx];
            _listOfItems.RemoveAt(idx);
            listViewItem.transform.SetParent(null);
            _poolOfItems.Despawn(listViewItem.gameObject);
        }

        public void Clear()
        {
            foreach (var item in _listOfItems)
            {
                item.transform.SetParent(null);
                _poolOfItems.Despawn(item.gameObject);
            }

            _listOfItems.Clear();
        }

        public IEnumerator<ListBoxItem> GetEnumerator() => _listOfItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}