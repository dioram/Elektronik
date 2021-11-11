using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    public class EnumField : SettingsFieldBase
    {
        [SerializeField] private TMP_Dropdown Dropdown;

        public IObservable<int> OnValueChanged()
        {
            return Dropdown.OnValueChangedAsObservable();
        }

        public void Setup(string labelText, string tooltip, Type enumType, int defaultValue)
        {
            SetupText(labelText, tooltip);
            _enumType = enumType;

            _enumValues = Enum.GetValues(_enumType).Cast<object>()
                    .Select(t => t.ToString())
                    .Select(s => new TMP_Dropdown.OptionData(s))
                    .ToList();
            Dropdown.options = _enumValues;
            Dropdown.value = defaultValue;
        }

        #region Private

        private List<TMP_Dropdown.OptionData> _enumValues;
        private Type _enumType;

        #endregion
    }
}