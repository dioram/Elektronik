using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.Protobuf.Offline.Presenters;

namespace Elektronik.Protobuf.Data
{
    public class ProtobufContainerTree : IDataSource, IVisibleDataSource
    {
        public readonly ITrackedCloudContainer<SlamTrackedObject> TrackedObjs;
        public readonly IConnectableObjectsCloudContainer<SlamObservation> Observations;
        public readonly IConnectableObjectsCloudContainer<SlamPoint> Points;
        public readonly ICloudContainer<SlamLine> Lines;
        public readonly ICloudContainer<SlamPlane> Planes;
        public readonly ICloudContainer<SlamMarker> Markers;
        public readonly ImagePresenter Image;
        public readonly IMeshContainer MeshContainer;
        public readonly Connector Connector;
        public readonly SlamDataInfoPresenter? SpecialInfo;

        public ProtobufContainerTree(string displayName, SlamDataInfoPresenter? specialInfo = null)
        {
            DisplayName = displayName;
            SpecialInfo = specialInfo;

            Image = new ImagePresenter("Camera");
            TrackedObjs = new TrackedCloudObjectsContainer("Tracked objects");
            Observations = new ConnectableObjectsCloudContainer<SlamObservation>(
                new CloudContainer<SlamObservation>("Points"),
                new SlamLinesCloudContainer("Connections"),
                "Observations");
            Points = new ConnectableObjectsCloudContainer<SlamPoint>(
                new CloudContainer<SlamPoint>("Points"),
                new SlamLinesCloudContainer("Connections"),
                "Points");
            Connector = new Connector(Points, Observations, "Connections");
            Lines = new SlamLinesCloudContainer("Lines");
            MeshContainer = new MeshReconstructor(Points, "Mesh");
            Planes = new CloudContainer<SlamPlane>("Planes");
            Markers = new CloudContainer<SlamMarker>("Markers");
            var observationsGraph = new VirtualDataSource("Observations graph", new List<IDataSource>
            {
                Observations,
                Connector
            });
            var ch = new List<IDataSource>
            {
                Points,
                TrackedObjs,
                Planes,
                observationsGraph,
                Lines,
                Markers,
                MeshContainer,
                Image,
            };
            if (SpecialInfo != null) ch.Add(SpecialInfo);
            Children = ch.ToArray();
        }

        #region IDataSource implementation

        public string DisplayName { get; set; }

        public IEnumerable<IDataSource> Children { get; }

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

        public IDataSource TakeSnapshot()
        {
            var children = new List<IDataSource>
            {
                (TrackedObjs as IDataSource)!.TakeSnapshot(),
                (Observations as IDataSource)!.TakeSnapshot(),
                (Points as IDataSource)!.TakeSnapshot(),
                (Lines as IDataSource)!.TakeSnapshot(),
                (Planes as IDataSource)!.TakeSnapshot(),
            };

            return new VirtualDataSource(DisplayName, children);
        }

        #endregion

        #region IVisibleDataSource

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                foreach (var child in Children.OfType<IVisibleDataSource>())
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