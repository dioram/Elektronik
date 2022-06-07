using Elektronik.DataSources;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;

namespace Elektronik.UI.SourceTreeControls
{
    internal class Header : MonoBehaviour
    {
        /// <summary> Link to UI object that contains link to data source. </summary>
        [SerializeField] private SourceTreeElement Parent;

        [SerializeField] private GameObject[] ButtonsPrefabs;
        [SerializeField] private Transform NameLabel;

        public IDataSource DataSource => Parent != null ? Parent.Node : null;

        private void Start()
        {
            foreach (var prefab in ButtonsPrefabs)
            {
                Instantiate(prefab, transform);
            }
            
            NameLabel.SetAsLastSibling();
            var nameText = NameLabel.GetComponent<TMP_Text>();
            nameText.SetLocalizedText(DataSource.DisplayName);
            if (DataSource is IColorfulDataSource c)
            {
                nameText.color = c.Color;
            }

            Parent.OnNameChanged += text => nameText.SetLocalizedText(text);
        }
    }
}