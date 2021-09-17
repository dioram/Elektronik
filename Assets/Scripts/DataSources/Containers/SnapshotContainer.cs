using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataSources.Containers.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    public class SnapshotContainer : ISourceTreeNode, IRemovable, IVisible, ISnapshotable, ISave
    {
        public SnapshotContainer(string displayName, IEnumerable<ISourceTreeNode> children)
        {
            DisplayName = displayName;
            Children = children;
        }

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTreeNode> Children { get; private set; }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }

            Children = Array.Empty<ISourceTreeNode>();
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
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
                                                 .Select(ch => ch as ISourceTreeNode)
                                                 .ToList());
        }

        #endregion

        #region Private

        private bool _isVisible = true;

        #endregion
    }
}