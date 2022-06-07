using Elektronik.DataSources.SpecialInterfaces;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> Filters data source by some weight. </summary>
    internal class WeightSlider : SourceTreeButton<IFilterableDataSource, Button>
    {
        #region Editor fields

        [SerializeField] private Slider Slider;
        [SerializeField] private TMP_Text MinWeightLabel;
        [SerializeField] private TMP_Text MaxWeightLabel;

        #endregion

        protected override void Initialize(IFilterableDataSource dataSource, Button uiButton)
        {
            void SetMaxWeight(int value)
            {
                MaxWeightLabel.text = $"{value}";
                Slider.maxValue = value;
            }

            dataSource.OnMaxWeightChanged += SetMaxWeight;
            SetMaxWeight(dataSource.MaxWeight);
            Slider.OnValueChangedAsObservable()
                    .Do(value => MinWeightLabel.text = $"{(int)value}")
                    .Subscribe(value => dataSource.MinWeight = (int)value);
        }
    }
}