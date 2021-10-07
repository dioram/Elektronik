using System.Collections.Generic;
using Elektronik.DataSources;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.UI
{
    public class DataSourcesTreeWidget : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private RectTransform SourceTreeView;
        [SerializeField] private GameObject TreeElementPrefab;

        #endregion
        
        public void AddDataSource(ISourceTreeNode source)
        {
            var treeElement = Instantiate(TreeElementPrefab, SourceTreeView).GetComponent<SourceTreeElement>();
            _roots.Add(treeElement);
            treeElement.Node = source;
            if (_roots.Count == 1)
            {
                treeElement.ChangeState();
            }

            if (source is IRemovable r)
            {
                r.OnRemoved += () =>
                {
                    Destroy(treeElement.gameObject);
                };
            }
        }

        public void RemoveDataSource(ISourceTreeNode source)
        {
            var treeElement = _roots.Find(r => r.Node == source);
            if (treeElement is null) return;
            _roots.Remove(treeElement);
            Destroy(treeElement.gameObject);
        }

        #region Private

        private readonly List<SourceTreeElement> _roots = new List<SourceTreeElement>();

        #endregion
    }
}