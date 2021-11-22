using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elektronik.UI
{
    /// <summary> Component for rendering tooltips. </summary>
    internal class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string TooltipText;
        public Vector2 Offset = new Vector2(0, 40);

        #region Unity events

        private void Start()
        {
            _tooltip = Instantiate(PrefabsStore.Instance.TooltipPrefab, transform);
            _label = _tooltip.GetComponentInChildren<TMP_Text>();
            _tooltip.SetActive(false);
            _tooltip.GetComponent<RectTransform>().anchoredPosition = Offset;
        }

        #endregion

        #region IPointer*Handler

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(TooltipText)) return;
            _isActive = true;
            _tooltip.SetActive(_isActive);
            _label.SetLocalizedText(TooltipText);
        }

        /// <inheritdoc />
        public void OnPointerExit(PointerEventData eventData)
        {
            _isActive = false;
            _tooltip.SetActive(_isActive);
        }

        #endregion

        #region Private
        
        private GameObject _tooltip;
        private bool _isActive;
        private TMP_Text _label;

        #endregion
    }
}