using System.Collections.Generic;
using Elektronik.DataSources;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.UI.SourceTreeControls;
using UnityEngine;

namespace Elektronik.UI
{
    public class DataSourcesTreeWidget : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private RectTransform SourceTreeView;
        [SerializeField] private GameObject TreeElementPrefab;

        #endregion
        
        public void AddDataSource(IDataSource dataSource)
        {
            var treeElement = Instantiate(TreeElementPrefab, SourceTreeView).GetComponent<SourceTreeElement>();
            _roots.Add(treeElement);
            treeElement.Node = dataSource;
            if (_roots.Count == 1)
            {
                treeElement.Expand();
            }

            if (dataSource is IRemovableDataSource r)
            {
                r.OnRemoved += () =>
                {
                    Destroy(treeElement.gameObject);
                };
            }
        }

        public void RemoveDataSource(IDataSource dataSource)
        {
            var treeElement = _roots.Find(r => r.Node == dataSource);
            if (treeElement is null) return;
            _roots.Remove(treeElement);
            Destroy(treeElement.gameObject);
        }

        #region Private

        private readonly List<SourceTreeElement> _roots = new List<SourceTreeElement>();

        #endregion
    }
}