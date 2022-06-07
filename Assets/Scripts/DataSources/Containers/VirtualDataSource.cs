using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataSources.SpecialInterfaces;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Class for virtual sources that can be used for grouping other sources in tree. </summary>
    public class VirtualDataSource : IVisibleDataSource
    {
        /// <summary> Constructor. </summary>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        /// <param name="children"> Children sources. </param>
        public VirtualDataSource(string displayName, IList<IDataSource> children = null)
        {
            DisplayName = displayName;
            ChildrenList = children ?? new List<IDataSource>();
        }

        /// <summary> Adds given source to this as child. </summary>
        public void AddChild(IDataSource child)
        {
            ChildrenList.Add(child);
        }

        #region IDataSource

        /// <inheritdoc />
        public virtual void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        /// <inheritdoc />
        public virtual void AddConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        /// <inheritdoc />
        public virtual void RemoveConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
            }
        }

        /// <inheritdoc />
        public IDataSource TakeSnapshot()
        {
            return new VirtualDataSource(DisplayName, ChildrenList
                                         .Select(ch => ch.TakeSnapshot())
                                         .Where(ch => ch is {})
                                         .ToList());
        }

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children => ChildrenList;

        #endregion

        #region IVisibleDataSource

        /// <inheritdoc />
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

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Protected
        
        /// <summary> List of children. </summary>
        protected readonly IList<IDataSource> ChildrenList;

        /// <summary> Removes unused virtual child nodes. </summary>
        protected void Squeeze()
        {
            for (int i = 0; i < ChildrenList.Count; i++)
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