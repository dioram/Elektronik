using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Offline.Presenters;

namespace Elektronik.Protobuf.Data
{
    public class ProtobufContainerTree : ISourceTree
    {
        public readonly ITrackedContainer<SlamTrackedObject> TrackedObjs;
        public readonly IConnectableObjectsContainer<SlamObservation> Observations;
        public readonly IConnectableObjectsContainer<SlamPoint> Points;
        public readonly IContainer<SlamLine> Lines;
        public readonly IContainer<SlamInfinitePlane> InfinitePlanes;
        public readonly ISourceTree Image;
        public readonly SlamDataInfoPresenter SpecialInfo;

        public ProtobufContainerTree(string displayName, ISourceTree image, SlamDataInfoPresenter specialInfo)
        {
            DisplayName = displayName;
            Image = image;
            SpecialInfo = specialInfo;

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
            var ch =  new List<ISourceTree>
            {
                (ISourceTree) Points,
                (ISourceTree) TrackedObjs,
                (ISourceTree) Observations,
                (ISourceTree) Lines,
                (ISourceTree) InfinitePlanes,
                Image,
            };
            if (SpecialInfo != null) ch.Add(SpecialInfo);
            Children = ch.ToArray();
        }

        private bool _isActive = true;

        #region ISourceTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; }

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