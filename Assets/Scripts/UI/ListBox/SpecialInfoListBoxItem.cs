using Elektronik.Cameras;
using Elektronik.Data.PackageObjects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.ListBox
{
    public class SpecialInfoListBoxItem : ListBoxItem
    {
        public TMP_Text MessageLabel;
        public ICloudItem Point;
        public TMP_Text Title;

        protected override void Start()
        {
            base.Start();
            Title.text = $"ID {Point.Id}. {Point.GetType().Name.Substring(4)}";
            ClickButton = GetComponent<Button>();
            ClickButton.OnClickAsObservable()
                    .Do(_ => MessageLabel.text = $"Item message: {Point.Message}")
                    .Select(_ => Camera.main.GetComponent<LookableCamera>())
                    .Where(c => c != null)
                    .Do(c => c.Look(Point.AsPoint().Look(c.transform)))
                    .Subscribe();
        }
    }
}

