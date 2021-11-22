using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Elektronik.UI.Localization
{
    /// <summary> Widget for selecting locale. </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public class LocaleSelector : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private List<Locale> Locales = new List<Locale>();

        #endregion

        #region Unity events

        public void Start()
        {
            var selector = GetComponent<TMP_Dropdown>();
            selector.options = Locales.Select(l => new TMP_Dropdown.OptionData(l.LocaleName)).ToList();
            // TODO: check not only player prefs
            var lastLocale = Locales.FirstOrDefault(l => l.LocaleName == PlayerPrefs.GetString("selected-locale"));
            if (lastLocale == null)
            {
                Debug.LogError("Last locale not found. Using default.");
                return;
            }
            LocalizationSettings.Instance.SetSelectedLocale(lastLocale);
            selector.value = Locales.IndexOf(lastLocale);
            selector.onValueChanged.AddListener(OnLocaleSelected);
        }

        #endregion

        #region Private

        private void OnLocaleSelected(int index)
        {
            LocalizationSettings.Instance.SetSelectedLocale(Locales[index]);
            PlayerPrefs.SetString("selected-locale", Locales[index].LocaleName);
            PlayerPrefs.Save();
        }

        #endregion
    }
}