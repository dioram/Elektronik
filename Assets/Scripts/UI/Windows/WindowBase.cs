using TMPro;
using UnityEngine;

namespace Elektronik.UI.Windows
{
    public class WindowBase : MonoBehaviour
    {
        #region Unity events

        protected void Start()
        {
            Title = transform.Find("Header/Title").GetComponent<TMP_Text>();
            Content = transform.Find("Content/Scroll View/Viewport/Content") as RectTransform;
        }

        #endregion

        #region Protected

        protected TMP_Text Title;
        protected RectTransform Content;

        #endregion
    }
}