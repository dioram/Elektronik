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
            try
            {
                var data = File.ReadAllLines(filename);
                var locales = data[0].Split(',');
                foreach (var str in data.Skip(1))
                {
                    var line = str.Split(',');
                    try
                    {
                        Translations.Add(line[0], new Dictionary<string, string>());
                        for (int i = 0; i < line.Length; i++)
                        {
                            Translations[line[0]][locales[i]] = line[i];
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when reading translation file: {e.Message}");
            }
        }

        public static void SetLocalizedText(this Text label, string text, IList<object> args = null)
        {
            label.text = GetLocalizedString(text, args);
        }

        public static void SetLocalizedText(this TMP_Text label, string text, IList<object> args = null)
        {
            label.text = GetLocalizedString(text, args);
        }

        public static string GetLocalizedString(string text, IList<object> args = null)
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