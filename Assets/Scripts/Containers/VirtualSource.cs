using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Renderers;

namespace Elektronik.Containers
{
    public class VirtualSource : ISourceTreeNode, IVisible, ISnapshotable
    {
        public VirtualSource(string displayName, List<ISourceTreeNode> children = null)
        {
            DisplayName = displayName;
            ChildrenList = children ?? new List<ISourceTreeNode>();
        }

        public void AddChild(ISourceTreeNode child)
        {
            ChildrenList.Add(child);
        }

        #region ISourceTree

        public virtual void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public virtual void AddRenderer(ISourceRenderer renderer)
        {
            foreach (var child in Children)
            {
                child.AddRenderer(renderer);
            }
        }

        public virtual void RemoveRenderer(ISourceRenderer renderer)
        {
            foreach (var child in Children)
            {
                child.RemoveRenderer(renderer);
            }
        }

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTreeNode> Children => ChildrenList;

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = IsVisible;
                }
            }
        }

        public bool ShowButton { get; private set; } = true;
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Protected

        protected void Squeeze()
        {
            for (int i = 0; i < ChildrenList.Count(); i++)
            {
                if (!(ChildrenList[i] is VirtualSource @virtual)) continue;

                @virtual.Squeeze();
                if (@virtual.ChildrenList.Count != 1) continue;

                ChildrenList[i] = @virtual.ChildrenList[0];
                ChildrenList[i].DisplayName = $"{@virtual.DisplayName}/{ChildrenList[i].DisplayName}";
            }

            ShowButton = CheckShowButton();
        }

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            return new VirtualSource(DisplayName, ChildrenList.OfType<ISnapshotable>()
                                                .Select(ch => ch.TakeSnapshot())
                                                .Select(ch => ch as ISourceTreeNode)
                                                .ToList());
        }

        #endregion

        #region Private

        private bool _isVisible = true;
        protected readonly List<ISourceTreeNode> ChildrenList;

        private bool CheckShowButton()
        {
            foreach (var child in ChildrenList)
            {
                switch (child)
                {
                case IVisible {ShowButton: true}:
                    ShowButton = true;
                    return true;
                case VirtualSource v when v.CheckShowButton():
                    ShowButton = true;
                    return true;
                }
            }

            ShowButton = false;
            return false;
        }

        #endregion
    }
}