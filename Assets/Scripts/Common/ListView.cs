using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common
{
    [RequireComponent(typeof(ScrollRect))]
    public class ListView : MonoBehaviour
    {
        Stack<GameObject> m_listViewItems;
        ScrollRect m_scrollView;

        public GameObject listViewItemPrefab;
        // Use this for initialization
        void Awake()
        {
            m_listViewItems = new Stack<GameObject>();
        }

        private void Start()
        {
            m_scrollView = GetComponent<ScrollRect>();
        }

        public void PushItem(string content)
        {
            GameObject newItem = Instantiate(listViewItemPrefab);
            newItem.transform.SetParent(m_scrollView.content);
            newItem.GetComponent<ListViewItem>().SetText(content);
            m_listViewItems.Push(newItem);
        }

        public void PopItem()
        {
            Destroy(m_listViewItems.Pop());
        }

        public void Clear()
        {
            while (m_listViewItems.Count != 0)
            {
                Destroy(m_listViewItems.Pop());
            }
        }
    }
}