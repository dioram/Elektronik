using System;
using System.IO;
using System.Linq;
using Elektronik.Settings;
using UnityEngine;

namespace Csv
{
    public class CsvFileSettingsBag : SettingsBag
    {
        [Tooltip("Paths to files with trajectories separated by semicolon(;). Each file - one trajectory.")]
        [CheckForEquals]
        public string PathsToFiles = "";

        public enum SeparatorType
        {
            Comma,
            Space,
            Semicolon,
            Tab
        }

        [CheckForEquals] public SeparatorType Separator = SeparatorType.Comma;

        public override ValidationResult Validate()
        {
            var notFoundPath = PathsToFiles.Split(';').FirstOrDefault(path => !File.Exists(path));
            return notFoundPath is null
                    ? ValidationResult.Succeeded
                    : ValidationResult.Failed($"Path {notFoundPath} not found.");
        }
    }

    public static class SeparatorExt
    {
        public static char GetSeparator(this CsvFileSettingsBag.SeparatorType separatorType)
            => separatorType switch
            {
                CsvFileSettingsBag.SeparatorType.Comma => ',',
                CsvFileSettingsBag.SeparatorType.Space => ' ',
                CsvFileSettingsBag.SeparatorType.Semicolon => ';',
                CsvFileSettingsBag.SeparatorType.Tab => '\t',
                _ => throw new ArgumentOutOfRangeException(nameof(separatorType), separatorType, null)
            };
    }
}