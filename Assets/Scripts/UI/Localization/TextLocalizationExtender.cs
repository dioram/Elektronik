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
            try
            {
                var defaultTable = LocalizationSettings.Instance.GetStringDatabase().GetDefaultTableAsync().Result;
                var localizedTitle = defaultTable.GetEntry(text).GetLocalizedString(args);
                label.text = localizedTitle;
            }
            catch
            {
                if (Translations.ContainsKey(text) && Translations[text].ContainsKey(GetCurrentLocale()))
                {
                    label.text = Translations[text][GetCurrentLocale()];
                }
                else
                {
                    label.text = text;
                }
            }
        }

        public static void SetLocalizedText(this TMP_Text label, string text, IList<object> args = null)
        {
            try
            {
                var defaultTable = LocalizationSettings.Instance.GetStringDatabase().GetDefaultTableAsync().Result;
                var localizedTitle = defaultTable.GetEntry(text).GetLocalizedString(args);
                label.text = localizedTitle;
            }
            catch
            {
                if (Translations.ContainsKey(text) && Translations[text].ContainsKey(GetCurrentLocale()))
                {
                    label.text = Translations[text][GetCurrentLocale()];
                }
                else
                {
                    label.text = text;
                }
            }
        }

        private static string GetCurrentLocale() => LocalizationSettings.Instance.GetSelectedLocale().LocaleName;

        private static readonly Dictionary<string, Dictionary<string, string>> Translations =
                new Dictionary<string, Dictionary<string, string>>();
    }
}