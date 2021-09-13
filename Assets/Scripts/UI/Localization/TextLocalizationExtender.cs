using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Elektronik.UI.Localization
{
    public static class TextLocalizationExtender
    {
        public static void ImportTranslations(string filename)
        {
            if (!File.Exists(filename)) return;
            var data = File.ReadAllLines(filename);
            var locales = data[0].Split(',');
            foreach (var str in data.Skip(1))
            {
                var line = str.Split(',');
                if (Translations.ContainsKey(line[0])) continue;
                Translations.Add(line[0], new Dictionary<string, string>());
                for (var i = 0; i < line.Length; i++)
                {
                    Translations[line[0]][locales[i]] = line[i];
                }
            }
        }

        public static void SetLocalizedText(this Text label, string text, params object[] args)
        {
            label.text = text.tr(args);
        }

        public static void SetLocalizedText(this TMP_Text label, string text, params object[] args)
        {
            label.text = text.tr(args);
        }

        // ReSharper disable once InconsistentNaming
        public static string tr(this string text, params object[] args)
        {
            var defaultTable = LocalizationSettings.Instance.GetStringDatabase().GetDefaultTableAsync().Result;
            if (defaultTable != null)
            {
                var entry = defaultTable.GetEntry(text);
                if (entry != null)
                {
                    var localizedTitle = entry.GetLocalizedString(args);
                    return localizedTitle;
                }
            }
            
            if (Translations.ContainsKey(text) && Translations[text].ContainsKey(GetCurrentLocale()))
            {
                return Translations[text][GetCurrentLocale()];
            }
            
            return text;
        }

        private static string GetCurrentLocale() => LocalizationSettings.Instance.GetSelectedLocale().LocaleName;

        private static readonly Dictionary<string, Dictionary<string, string>> Translations =
                new Dictionary<string, Dictionary<string, string>>();
    }
}