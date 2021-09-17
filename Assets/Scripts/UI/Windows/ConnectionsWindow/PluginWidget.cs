using System;
using Elektronik.PluginsSystem;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.UI.Windows
{
    [RequireComponent(typeof(LayoutElement))]
    public class PluginWidget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Button HeaderButton;
        [SerializeField] private TMP_Text HeaderLabel;
        [SerializeField] private TMP_Text DescriptionLabel;
        [SerializeField] private RectTransform Description;
        [SerializeField] private RawImage LogoLabel;
        [Range(30, 100), SerializeField] private int Height = 50;
        [Range(30, 1000), SerializeField] private int HeightWithDescription = 150;

        private bool _isExpanded = false;
        private LayoutElement _layoutElement;
        public IDataSourcePluginsFactory Plugin { get; private set; }

        public void Setup(IDataSourcePluginsFactory plugin)
        {
            Plugin = plugin;
        }

        public IObservable<IDataSourcePluginsFactory> OnSelected() =>
                HeaderButton.OnClickAsObservable().Where(_ => !_isExpanded).Select(_ => Plugin);

        public void Expand()
        {
            _isExpanded = true;
            Description.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HeightWithDescription - Height);
            ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HeightWithDescription);
            _layoutElement.minHeight = HeightWithDescription;
        }

        public void Minimize()
        {
            _isExpanded = false;
            Description.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
            _layoutElement.minHeight = Height;
        }

        public void Select()
        {
            HeaderButton.OnPointerClick(new PointerEventData(null));
        }

        private void Start()
        {
            _layoutElement = GetComponent<LayoutElement>();
            OnSelected().Subscribe(_ => Expand()).AddTo(this);
            HeaderLabel.SetLocalizedText(Plugin.DisplayName);
            DescriptionLabel.SetLocalizedText(Plugin.Description);
            if (Plugin.Logo is null) LogoLabel.enabled = false;
            else LogoLabel.texture = Plugin.Logo;
            Minimize();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(DescriptionLabel, eventData.position,
                                                                   eventData.pressEventCamera);
            if (linkIndex == -1) return;
            var linkInfo = DescriptionLabel.textInfo.linkInfo[linkIndex];
            var selectedLink = linkInfo.GetLinkID();

            if (selectedLink == "") return;
            Application.OpenURL(selectedLink);
        }
    }
}