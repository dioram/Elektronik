using UnityEngine;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> Base class for all buttons near data source in tree. </summary>
    /// <typeparam name="TInterface"> Type of interface controller by button. </typeparam>
    /// <typeparam name="TUiType"> Type of button. </typeparam>
    internal abstract class SourceTreeButton<TInterface, TUiType> : MonoBehaviour
    {
        /// <summary> Link to UI object that contains link to data source. </summary>
        [SerializeField] protected SourceTreeElement Parent;
        
        protected TUiType UiButton;
        protected TInterface DataSource;

        /// <summary> Initializes this button. </summary>
        /// <param name="dataSource"></param>
        /// <param name="uiButton"></param>
        protected abstract void Initialize(TInterface dataSource, TUiType uiButton);

        #region Unity events

        private void Start()
        {
            var ds = GetComponentInParent<Header>().DataSource;

            UiButton = GetComponent<TUiType>();
            if (ds is TInterface i)
            {
                DataSource = i;
                Initialize(DataSource, UiButton);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}