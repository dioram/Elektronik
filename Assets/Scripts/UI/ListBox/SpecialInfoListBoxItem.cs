using Elektronik.Cameras;
using Elektronik.Data.PackageObjects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.ListBox
{
    [RequireComponent(typeof(Button))]
    public class SpecialInfoListBoxItem : ListBoxItem
    {
        public TMP_Text MessageLabel;
        public ICloudItem Point;
        public TMP_Text Title;

        private Button _button;

        protected override void Start()
        {
            base.Start();
            Title.text = $"ID {Point.Id}. {Point.GetType().Name.Substring(4)}";
            _button = GetComponent<Button>();
            _button.OnClickAsObservable()
                    .Do(_ => MessageLabel.text = $"Item message: {Point.Message}")
                    .Select(_ => Camera.main.GetComponent<LookableCamera>())
                    .Where(c => c != null)
                    .Do(c => c.Look(Point.AsPoint().Look(c.transform)))
                    .Subscribe();
        }
    }
}

