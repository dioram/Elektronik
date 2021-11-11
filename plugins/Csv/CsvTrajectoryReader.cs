using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common;
using Elektronik.PluginsSystem;
using Elektronik.Settings;
using UnityEngine;

namespace Csv
{
    public class CsvTrajectoryReader : IDataSourcePlugin
    {
        public CsvTrajectoryReader(CsvFileSettingsBag settings)
        {
            var container = new TrackedObjectsContainer(Path.GetFileName(settings.PathsToFiles.Split(';').First()));
            Data = container;
            _settings = settings;
            foreach (var (file, i) in settings.PathsToFiles.Split(';').Select((s, i) => (s, i)))
            {
                ReadTrajectory(file!, i, settings.Separator.GetSeparator(), container);
            }
        }

        public void Dispose()
        {
            Data.Clear();
        }

        public void Update(float delta)
        {
            // Do noting
        }

        public string DisplayName => "CSV";
        public SettingsBag? Settings => null;
        public Texture2D? Logo { get; }
        public ISourceTreeNode Data { get; }

        #region Private

        private readonly CsvFileSettingsBag _settings;

        private void ReadTrajectory(string filename, int id, char separator, TrackedObjectsContainer container)
        {
            var firstLine = true;
            var converter = new RightHandToLeftHandConverter();
            foreach (var line in File.ReadAllLines(filename))
            {
                if (!TryGetPose(line, separator, out var pos, out var rot)) continue;
                converter.Convert(ref pos, ref rot);
                if (firstLine)
                {
                    firstLine = false;
                    container.Add(new SlamTrackedObject(id, pos, rot));
                    continue;
                }

                container.Update(new SlamTrackedObject(id, pos, rot));
            }
        }

        private bool TryGetPose(string line, char separator, out Vector3 pos, out Quaternion rot)
        {
            float[] values;
            try
            {
                values = line.Split(separator)
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Select(v => double.Parse(v, CultureInfo.InvariantCulture))
                        .Select(v => (float)v)
                        .ToArray();
            }
            catch (Exception)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
                return false;
            }

            if (values.Length == 3)
            {
                pos = new Vector3(values[1], 0, values[2]);
                rot = Quaternion.identity;
                return true;
            }

            if (values.Length < 4)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
                return false;
            }

            pos = new Vector3(values[1], values[2], values[3]);

            if (values.Length < 8)
            {
                rot = Quaternion.identity;
                return true;
            }

            rot = new Quaternion(values[5], values[6], values[7], values[4]);
            return true;
        }

        #endregion
    }
}