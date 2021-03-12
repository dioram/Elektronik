using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;

namespace Elektronik.Rosbag2.Containers
{
    public class VirtualContainer : IContainerTree
    {
        public VirtualContainer(string displayName)
        {
            DisplayName = displayName;
        }

        public void Squeeze()
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

        public void AddChild(IContainerTree child)
        {
            ChildrenList.Add(child);
        }

        public IEnumerable<IContainerTree> GetRealChildren()
        {
            var res = new List<IContainerTree>();
            foreach (var child in Children)
            {
                if (child is VirtualContainer @virtual)
                {
                    res.AddRange(@virtual.GetRealChildren());
                }
                else
                {
                    res.Add(child);
                }
            }

            return res;
        }

        public virtual string GetFullPath(IContainerTree container)
        {
            foreach (var child in Children)
            {
                var @virtual = child as VirtualContainer;
                if (@virtual == null && container == child)
                {
                    return $"{DisplayName}/{child.DisplayName}";
                }

                if (@virtual?.GetFullPath(container) is { } path)
                {
                    return $"{DisplayName}/{path}";
                }
            }

            return null;
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

        public IEnumerable<IContainerTree> Children => ChildrenList;

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

        #region Private definitions

        private bool _isActive = true;
        protected readonly List<IContainerTree> ChildrenList = new List<IContainerTree>();

        #endregion
    }
}