using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Mesh;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Offline.Presenters;

namespace Elektronik.Protobuf.Data
{
    public class ProtobufContainerTree : ISourceTree, ISnapshotable, IVisible
    {
        public readonly ITrackedContainer<SlamTrackedObject> TrackedObjs;
        public readonly IConnectableObjectsContainer<SlamObservation> Observations;
        public readonly IConnectableObjectsContainer<SlamPoint> Points;
        public readonly IContainer<SlamLine> Lines;
        public readonly IContainer<SlamInfinitePlane> InfinitePlanes;
        public readonly ISourceTree Image;
        public readonly IMeshContainer MeshContainer;
        public readonly Connector Connector;
        public readonly SlamDataInfoPresenter SpecialInfo;

        public ProtobufContainerTree(string displayName, ISourceTree image, SlamDataInfoPresenter specialInfo = null, bool drawMesh = true)
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
            Connector = new Connector(Points, Observations, "Connections");

            bool meshImplemented = true;
            try
            {
                if (drawMesh) MeshContainer = new MeshReconstructor(Points, Observations, "Mesh");
            }
            catch (NotImplementedException e)
            {
                meshImplemented = false;
            }

            Lines = new SlamLinesContainer("Lines");
            InfinitePlanes = new CloudContainer<SlamInfinitePlane>("Infinite planes");
            var observationsGraph = new VirtualContainer("Observations graph", new List<ISourceTree>
            {
                (ISourceTree) Observations,
                Connector
            });
            var ch =  new List<ISourceTree>
            {
                (ISourceTree) Points,
                (ISourceTree) TrackedObjs,
                (ISourceTree) InfinitePlanes,
                observationsGraph,
                (ISourceTree) Lines,
                Image,
            };
            if (drawMesh && meshImplemented) ch.Add(MeshContainer);
            if (SpecialInfo != null) ch.Add(SpecialInfo);
            Children = ch.ToArray();
        }

        #region ISourceTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public void SetRenderer(ISourceRenderer renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var children = new List<ISourceTree>()
            {
                (TrackedObjs as ISnapshotable)!.TakeSnapshot() as ISourceTree,
                (Observations as ISnapshotable)!.TakeSnapshot() as ISourceTree,
                (Points as ISnapshotable)!.TakeSnapshot() as ISourceTree,
                (Lines as ISnapshotable)!.TakeSnapshot() as ISourceTree,
                (InfinitePlanes as ISnapshotable)!.TakeSnapshot() as ISourceTree,
            };
            
            return new VirtualContainer(DisplayName, children);
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            foreach (var snapshotable in Children.OfType<ISnapshotable>())
            {
                snapshotable.WriteSnapshot(recorder);
            }
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = value;
                }
                OnVisibleChanged?.Invoke(value);
            }
        }

        public bool ShowButton { get; } = true;
        
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private

        private bool _isVisible = true;

        #endregion
    }
}