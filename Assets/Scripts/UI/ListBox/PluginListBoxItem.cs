using System;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.UI.ListBox
{
    public class PluginListBoxItem : ListBoxItem, IPointerClickHandler
    {
        [Range(30, 100)] public int Height = 50;
        [Range(30, 1000)] public int HeightWithDescription = 300;

        public IElektronikPlugin Plugin;
        public bool State => _toggle.isOn;

        private RectTransform _description;
        private Toggle _toggle;
        private Text _label;
        private TMP_Text _descriptionLabel;
        private LayoutElement _layoutElement;

        public IObservable<bool> OnValueChangedAsObservable() => _toggle.OnValueChangedAsObservable();

        public void Toggle(bool state)
        {
            _toggle.isOn = state;
        }

        protected override void Awake()
        {
            base.Awake();
            _description = (RectTransform) transform.Find("Description");
            _toggle = transform.Find("Toggle").GetComponent<Toggle>();
            _label = transform.Find("Text").GetComponent<Text>();
            _descriptionLabel = _description.Find("Text").GetComponent<TMP_Text>();
            _layoutElement = GetComponent<LayoutElement>();
        }

        protected override void Start()
        {
            base.Start();
            _label.text = $"{Plugin.DisplayName} (v{Plugin.GetVersion()})";
            _descriptionLabel.text = Plugin.Description;

            _toggle.OnValueChangedAsObservable()
                    .Where(state => state)
                    .Subscribe(_ => PluginsLoader.EnablePlugin(Plugin));
            _toggle.OnValueChangedAsObservable()
                    .Where(state => !state)
                    .Subscribe(_ => PluginsLoader.DisablePlugin(Plugin));
        }

        public void ToggleDescription()
        {
            if (_description.rect.height == 0)
            {
                ((RectTransform) _description.transform)
                        .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                                                   HeightWithDescription - Height);
                ((RectTransform) transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                                                                      HeightWithDescription);
                _layoutElement.minHeight = HeightWithDescription;
            }
            else
            {
                ((RectTransform) _description.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                ((RectTransform) transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
                _layoutElement.minHeight = Height;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_descriptionLabel, eventData.position,
                                                                   eventData.pressEventCamera);
            if (linkIndex == -1) return;
            var linkInfo = _descriptionLabel.textInfo.linkInfo[linkIndex];
            string selectedLink = linkInfo.GetLinkID();
            
            if (selectedLink == "") return;
            Application.OpenURL(selectedLink);
        }
    }
}