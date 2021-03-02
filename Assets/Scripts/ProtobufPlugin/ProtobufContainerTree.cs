using System.Collections.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.ProtobufPlugin
{
    public class ProtobufContainerTree : IContainerTree
    {
        public readonly ITrackedContainer<SlamTrackedObject> TrackedObjs;
        public readonly IConnectableObjectsContainer<SlamObservation> Observations;
        public readonly IConnectableObjectsContainer<SlamPoint> Points;
        public readonly IContainer<SlamLine> Lines;
        public readonly IContainer<SlamInfinitePlane> InfinitePlanes;

        public ProtobufContainerTree(string displayName)
        {
            DisplayName = displayName;

            TrackedObjs = new TrackedObjectsContainer("Tracked objects");
            Observations = new ConnectableObjectsContainer<SlamObservation>(
                new CloudContainer<SlamObservation>("Points"),
                new SlamLinesContainer("Connections"),
                "Observations");
            Points = new ConnectableObjectsContainer<SlamPoint>(
                new CloudContainer<SlamPoint>("Points"),
                new SlamLinesContainer("Connections"),
                "Points");

            Lines = new SlamLinesContainer("Lines");
            InfinitePlanes = new CloudContainer<SlamInfinitePlane>("Infinite planes");

            Children = new[]
            {
                (IContainerTree) Points,
                (IContainerTree) TrackedObjs,
                (IContainerTree) Observations,
                (IContainerTree) Lines,
                (IContainerTree) InfinitePlanes,
            };
        }

        private bool _isActive = true;

        #region IContainerTree implementation

        public string DisplayName { get; set; }

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
    }
}