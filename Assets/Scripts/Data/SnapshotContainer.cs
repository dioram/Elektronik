using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleFileBrowser;

namespace Elektronik.Data
{
    public class SnapshotContainer : ISourceTree, IRemovable, IVisible, ISnapshotable, ISave
    {
        public SnapshotContainer(string displayName, IEnumerable<ISourceTree> children)
        {
            DisplayName = displayName;
            Children = children;
        }

        public static SnapshotContainer Load(string filename)
        {
            var serializer = new JsonSerializer();
            using var file = File.OpenText(filename);
            using var jsonTextReader = new JsonTextReader(file);
            var arr = serializer.Deserialize<JToken>(jsonTextReader)["data"];

            return new SnapshotContainer($"Snapshot: {Path.GetFileName(filename)}",
                                         arr.Where(t => t.HasValues)
                                                 .Select(SnapshotableDeserializer.Deserialize).ToList());
        }

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; private set; }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }

            Children = new ISourceTree[0];
        }

        public void SetRenderer(object renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        #endregion

        #region IRemovable

        public void RemoveSelf()
        {
            Clear();
            OnRemoved?.Invoke();
        }

        public event Action OnRemoved;

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = value;
                }

                OnVisibleChanged?.Invoke(value);
            }
        }

        public event Action<bool> OnVisibleChanged;
        public bool ShowButton { get; } = true;

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            return new SnapshotContainer(DisplayName, Children
                                                 .OfType<ISnapshotable>()
                                                 .Select(ch => ch.TakeSnapshot())
                                                 .Select(ch => ch as ISourceTree)
                                                 .ToList());
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            foreach (var snapshotable in Children.OfType<ISnapshotable>())
            {
                snapshotable.WriteSnapshot(recorder);
            }
        }

        #endregion

        #region ISave

        public void Save()
        {
            var recorders = PluginsLoader.Plugins.Value.OfType<IDataRecorderPlugin>().ToList();
            FileBrowser.SetFilters(false, recorders.Select(r => r.Extension));
            FileBrowser.ShowSaveDialog(path => Save(path[0]),
                                       () => { },
                                       false,
                                       false,
                                       "",
                                       TextLocalizationExtender.GetLocalizedString("Save snapshot"),
                                       TextLocalizationExtender.GetLocalizedString("Save"));
        }

        #endregion

        #region Private

        private bool _isVisible = true;

        private void Save(string filename)
        {
            var recorder = PluginsLoader.Plugins.Value
                    .OfType<IDataRecorderPlugin>()
                    .First(r => filename.EndsWith(r.Extension));
            recorder.FileName = filename;
            recorder.StartRecording();
            WriteSnapshot(recorder);
            recorder.StopRecording();
        }

        #endregion
    }
}