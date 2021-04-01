using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class VirtualContainer : ISourceTree
    {
        public VirtualContainer(string displayName)
        {
            DisplayName = displayName;
        }

        public void AddChild(ISourceTree child)
        {
            ChildrenList.Add(child);
        }

        #region IContainerTree implementation

        public virtual void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public virtual void SetRenderer(object renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => ChildrenList;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                foreach (var child in Children)
                {
                    child.IsActive = IsActive;
                }
            }
        }

        #endregion

        #region Protected

        protected void Squeeze()
        {
            for (int i = 0; i < ChildrenList.Count(); i++)
            {
                if (ChildrenList[i] is not VirtualContainer @virtual) continue;

                @virtual.Squeeze();
                if (@virtual.ChildrenList.Count != 1) continue;

                ChildrenList[i] = @virtual.ChildrenList[0];
                ChildrenList[i].DisplayName = $"{@virtual.DisplayName}/{ChildrenList[i].DisplayName}";
            }
        }

        #endregion

        #region Private definitions

        private bool _isActive = true;
        protected readonly List<ISourceTree> ChildrenList = new();

        #endregion
    }
}