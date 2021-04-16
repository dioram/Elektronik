using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Elektronik.UI.Localization
{
    [RequireComponent(typeof(Dropdown))]
    public class LocaleSelector : MonoBehaviour
    {
        private Dropdown _selector;
        [SerializeField] private List<Locale> Locales = new List<Locale>();
        
        public void Start()
        {
            _selector = GetComponent<Dropdown>();
            _selector.options = Locales.Select(l => new Dropdown.OptionData(l.LocaleName)).ToList();
            var lastLocale = Locales.FirstOrDefault(l => l.LocaleName == PlayerPrefs.GetString("selected-locale"));
            if (lastLocale == null) return;
            LocalizationSettings.Instance.SetSelectedLocale(lastLocale);
            _selector.value = Locales.IndexOf(lastLocale);
        }
        
        public void OnLocaleSelected(int index)
        {
            LocalizationSettings.Instance.SetSelectedLocale(Locales[index]);
            PlayerPrefs.SetString("selected-locale", Locales[index].LocaleName);
            PlayerPrefs.Save();
        }
    }
}