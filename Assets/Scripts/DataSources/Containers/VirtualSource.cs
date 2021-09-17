using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataSources.Containers.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    public class VirtualSource : ISourceTreeNode, IVisible
    {
        public VirtualSource(string displayName, IList<ISourceTreeNode> children = null)
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

        public virtual void AddConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        public virtual void RemoveConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
            }
        }
        public ISourceTreeNode TakeSnapshot()
        {
            return new VirtualSource(DisplayName, ChildrenList
                                         .Select(ch => ch.TakeSnapshot())
                                         .Where(ch => ch is {})
                                         .ToList());
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

        #region Private

        private bool _isVisible = true;
        protected readonly IList<ISourceTreeNode> ChildrenList;

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