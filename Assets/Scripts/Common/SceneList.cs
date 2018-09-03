using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Elektronik.Common
{
    public class SceneList : MonoBehaviour
    {
        public GameObject listItemPrefab;
        private ScrollRect m_scrollView;
        void Start()
        {
            m_scrollView = GetComponentInChildren<ScrollRect>();
            FillScrollView();
        }

        void FillScrollView()
        {
            var contentOfScrollView = m_scrollView.content;
            string[] scenes = UnityEditor.EditorBuildSettings.scenes
                .Where(s => s.path.Contains(@"Scenes/Common/WorkArea"))
                .Select(s => s.path)
                .ToArray();
            if (scenes != null && scenes.Length != 0)
            {
                var listItems = scenes.Select(s => new { Item = Instantiate(listItemPrefab), Name = s });
                foreach (var listItem in listItems)
                {
                    ListViewItem item = listItem.Item.GetComponent<ListViewItem>();
                    item.FullPath = listItem.Name;
                    listItem.Item.transform.SetParent(contentOfScrollView);
                }
            }
        }
    }
}