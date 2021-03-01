using System.Collections.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.RandomDataPlugin
{
    public class RandomContainerTree : IContainerTree
    {
        public readonly IContainer<SlamPoint> Points = new CloudContainer<SlamPoint>(null);
        public readonly IContainer<SlamLine> Lines = new SlamLinesContainer();

        public RandomContainerTree()
        {
            Children = new[]
            {
                (IContainerTree) Points,
                (IContainerTree) Lines,
            };
        }

        #region IContainerTree implementation

        public string DisplayName => "RandomData";

        public IEnumerable<IContainerTree> Children { get; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                foreach (var child in Children)
                {
                    child.IsActive = value;
                }

                _isActive = value;
            }
        }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public void SetRenderer(object renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        #endregion

        #region Private definitions

        private bool _isActive = true;

        #endregion
    }
}