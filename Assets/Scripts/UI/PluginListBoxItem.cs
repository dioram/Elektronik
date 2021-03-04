using System;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class PluginListBoxItem : ListBoxItem
    {
        [Range(30, 100)] public int Height = 50;
        [Range(30, 1000)] public int HeightWithDescription = 300;

        public IElektronikPlugin Plugin;

        private RectTransform _description;
        private Toggle _toggle;
        private Text _label;
        private Text _descriptionLabel;
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
            _descriptionLabel = _description.Find("Text").GetComponent<Text>();
            _layoutElement = GetComponent<LayoutElement>();
        }

        protected override void Start()
        {
            base.Start();
            _label.text = Plugin.DisplayName;
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
    }
}