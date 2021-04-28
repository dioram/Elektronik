using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;

namespace Elektronik.Data
{
    public class SnapshotContainer : ISourceTree, IRemovable, IVisible
    {
        public SnapshotContainer(string displayName, IEnumerable<ISourceTree> children)
        {
            DisplayName = displayName;
            Children = children;
        }

        #region ISourceTree

        public string DisplayName { get; set; }
        
        public IEnumerable<ISourceTree> Children { get; private set; }
        
        public void Clear()
        {
            Children = new ISourceTree[0];
            // do nothing
        }

        public void SetRenderer(object renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        public ISourceTree ContainersOnlyCopy()
        {
            return new SnapshotContainer(DisplayName, Children.Select(ch => ch.ContainersOnlyCopy())
                                                 .Where(ch => ch != null));
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

        #region Private

        private bool _isVisible = true;

        #endregion
    }
}