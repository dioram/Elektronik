using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class VirtualContainer : ISourceTree, IVisible
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

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = IsVisible;
                }
            }
        }

        public bool ShowButton { get; private set; }

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

            ShowButton = CheckShowButton();
        }

        #endregion

        #region Private

        private bool _isVisible = true;
        protected readonly List<ISourceTree> ChildrenList = new();

        private bool CheckShowButton()
        {
            foreach (var child in ChildrenList)
            {
                switch (child)
                {
                case IVisible {ShowButton: true}:
                    ShowButton = true;
                    return true;
                case VirtualContainer v when v.CheckShowButton():
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