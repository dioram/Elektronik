using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.Protobuf.Offline.Presenters;

namespace Elektronik.Protobuf.Data
{
    public class ProtobufContainerTree : ISourceTreeNode, IVisible
    {
        public readonly ITrackedContainer<SlamTrackedObject> TrackedObjs;
        public readonly IConnectableObjectsContainer<SlamObservation> Observations;
        public readonly IConnectableObjectsContainer<SlamPoint> Points;
        public readonly IContainer<SlamLine> Lines;
        public readonly IContainer<SlamPlane> Planes;
        public readonly ISourceTreeNode? Image;
        public readonly IMeshContainer MeshContainer;
        public readonly Connector Connector;
        public readonly SlamDataInfoPresenter? SpecialInfo;

        public ProtobufContainerTree(string displayName, ISourceTreeNode? image, SlamDataInfoPresenter? specialInfo = null)
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
            Lines = new SlamLinesContainer("Lines");
            MeshContainer = new MeshReconstructor(Points, "Mesh");
            Planes = new CloudContainer<SlamPlane>("Planes");
            var observationsGraph = new VirtualSource("Observations graph", new List<ISourceTreeNode>
            {
                (ISourceTreeNode) Observations,
                Connector
            });
            var ch =  new List<ISourceTreeNode>
            {
                (ISourceTreeNode) Points,
                (ISourceTreeNode) TrackedObjs,
                (ISourceTreeNode) Planes,
                observationsGraph,
                (ISourceTreeNode) Lines,
                MeshContainer,
            };
            if (Image != null) ch.Add(Image);
            if (SpecialInfo != null) ch.Add(SpecialInfo);
            Children = ch.ToArray();
        }

        #region ISourceTreeNode implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTreeNode> Children { get; }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
            }
        }

        #endregion

        #region ISnapshotable

        public ISourceTreeNode TakeSnapshot()
        {
            var children = new List<ISourceTreeNode>
            {
                (TrackedObjs as ISourceTreeNode)!.TakeSnapshot(),
                (Observations as ISourceTreeNode)!.TakeSnapshot(),
                (Points as ISourceTreeNode)!.TakeSnapshot(),
                (Lines as ISourceTreeNode)!.TakeSnapshot(),
                (Planes as ISourceTreeNode)!.TakeSnapshot(),
            };
            
            return new VirtualSource(DisplayName, children);
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
        
        public event Action<bool>? OnVisibleChanged;

        #endregion

        #region Private

        private bool _isVisible = true;

        #endregion
    }
}