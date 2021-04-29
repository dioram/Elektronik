using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Newtonsoft.Json.Linq;

namespace Elektronik.Containers
{
    public class VirtualContainer : ISourceTree, IVisible, ISnapshotable
    {
        public VirtualContainer(string displayName, List<ISourceTree> children = null)
        {
            DisplayName = displayName;
            ChildrenList = children ?? new List<ISourceTree>();
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
                if (!(ChildrenList[i] is VirtualContainer @virtual)) continue;

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
            return new VirtualContainer(DisplayName, ChildrenList.OfType<ISnapshotable>()
                                                .Select(ch => ch.TakeSnapshot())
                                                .Select(ch => ch as ISourceTree)
                                                .ToList());
        }
        

        public string Serialize()
        {
            var data = string.Join(",", ChildrenList.OfType<ISnapshotable>().Select(ch => ch.Serialize()));
            return $"{{\"displayName\":\"{DisplayName}\",\"type\":\"virtual\",\"data\":[{data}]}}";
        }

        public static VirtualContainer Deserialize(JToken token)
        {
            return new VirtualContainer(token["displayName"].ToString(),
                                        token["data"].Where(t => t.HasValues)
                                                .Select(SnapshotableDeserializer.Deserialize).ToList());
        }

        #endregion

        #region Private

        private bool _isVisible = true;
        protected readonly List<ISourceTree> ChildrenList;

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