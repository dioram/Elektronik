using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.PluginsSystem.UnitySide;
using Elektronik.UI.Localization;
using SimpleFileBrowser;

namespace Elektronik.Data
{
    public class SnapshotContainer : ISourceTree, IRemovable, IVisible, ISnapshotable, ISave
    {
        public SnapshotContainer(string displayName, IEnumerable<ISourceTree> children, ICSConverter converter)
        {
            _converter = converter;
            DisplayName = displayName;
            Children = children;
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

        public void SetRenderer(ISourceRenderer renderer)
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
                                                 .ToList(),
                                         _converter);
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
        private readonly ICSConverter _converter;

        private void Save(string filename)
        {
            var recorder = PluginsPlayer.Plugins
                    .OfType<IDataRecorderPlugin>()
                    .First(r => filename.EndsWith(r.Extension));
            recorder.FileName = filename;
            recorder.Converter = _converter;
            recorder.StartRecording();
            WriteSnapshot(recorder);
            recorder.StopRecording();
        }

        #endregion
    }
}