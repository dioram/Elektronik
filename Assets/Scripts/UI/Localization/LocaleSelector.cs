using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Elektronik.UI.Localization
{
    public class LocaleSelector : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown Selector;
        [SerializeField] private List<Locale> Locales = new List<Locale>();
        
        public void Start()
        {
            Selector.options = Locales.Select(l => new TMP_Dropdown.OptionData(l.LocaleName)).ToList();
            var lastLocale = Locales.FirstOrDefault(l => l.LocaleName == PlayerPrefs.GetString("selected-locale"));
            if (lastLocale == null)
            {
                Debug.LogError("Last locale not found. Using default.");
                return;
            }
            LocalizationSettings.Instance.SetSelectedLocale(lastLocale);
            Selector.value = Locales.IndexOf(lastLocale);
        }
        
        public void OnLocaleSelected(int index)
        {
            LocalizationSettings.Instance.SetSelectedLocale(Locales[index]);
            PlayerPrefs.SetString("selected-locale", Locales[index].LocaleName);
            PlayerPrefs.Save();
        }
    }
}