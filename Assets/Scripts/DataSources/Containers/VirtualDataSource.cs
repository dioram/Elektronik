using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    public class VirtualDataSource : IVisibleDataSource
    {
        public VirtualDataSource(string displayName, IList<IDataSource> children = null)
        {
            DisplayName = displayName;
            ChildrenList = children ?? new List<IDataSource>();
        }

        public void AddChild(IDataSource child)
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
        public IDataSource TakeSnapshot()
        {
            return new VirtualDataSource(DisplayName, ChildrenList
                                         .Select(ch => ch.TakeSnapshot())
                                         .Where(ch => ch is {})
                                         .ToList());
        }

        public string DisplayName { get; set; }

        public IEnumerable<IDataSource> Children => ChildrenList;

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
                foreach (var child in Children.OfType<IVisibleDataSource>())
                {
                    child.IsVisible = IsVisible;
                }
            }
        }
        
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Protected
        
        protected readonly IList<IDataSource> ChildrenList;

        protected void Squeeze()
        {
            for (int i = 0; i < ChildrenList.Count(); i++)
            {
                if (!(ChildrenList[i] is VirtualDataSource @virtual)) continue;

                @virtual.Squeeze();
                if (@virtual.ChildrenList.Count != 1) continue;

                ChildrenList[i] = @virtual.ChildrenList[0];
                ChildrenList[i].DisplayName = $"{@virtual.DisplayName}/{ChildrenList[i].DisplayName}";
            }

        }

        #endregion

        #region Private

        private bool _isVisible = true;

        #endregion
    }
}