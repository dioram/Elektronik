using System.Collections.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using SlamLinesContainer = Elektronik.Common.Containers.NotMono.SlamLinesContainer;
using TrackedObjectsContainer = Elektronik.Common.Containers.NotMono.TrackedObjectsContainer;

namespace Elektronik.ProtobufPlugin
{
    public class ProtobufContainerTree : IContainerTree
    {
        public readonly ITrackedContainer<SlamTrackedObject> TrackedObjs;
        public readonly IConnectableObjectsContainer<SlamObservation> Observations;
        public readonly IConnectableObjectsContainer<SlamPoint> Points;
        public readonly ILinesContainer<SlamLine> Lines;
        public readonly IContainer<SlamInfinitePlane> InfinitePlanes;
        
        public ProtobufContainerTree(string displayName)
        {
            DisplayName = displayName;

            TrackedObjs = new TrackedObjectsContainer("Tracked objects");
            Observations = new Elektronik.Common.Containers.NotMono.ConnectableObjectsContainer<SlamObservation>(
                    new Elektronik.Common.Containers.NotMono.CloudContainer<SlamObservation>("Points"),
                    new SlamLinesContainer("Connections"),
                    "Observations");
            Points = new Elektronik.Common.Containers.NotMono.ConnectableObjectsContainer<SlamPoint>(
                    new Elektronik.Common.Containers.NotMono.CloudContainer<SlamPoint>("Points"),
                    new SlamLinesContainer("Connections"),
                    "Points");

            Lines = new SlamLinesContainer("Lines");
            InfinitePlanes = new Elektronik.Common.Containers.NotMono.CloudContainer<SlamInfinitePlane>("Infinite planes");

            Children = new[]
            {
                    (IContainerTree) Points,
                    (IContainerTree) TrackedObjs,
                    (IContainerTree) Observations,
                    (IContainerTree) Lines,
                    (IContainerTree) InfinitePlanes,
            };
        }

        private bool _isActive;

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