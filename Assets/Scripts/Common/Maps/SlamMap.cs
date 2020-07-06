using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using UnityEngine;

namespace Elektronik.Common.Maps
{
    public class SlamMap : MonoBehaviour
    {
        public Helmet helmetPrefab;

        private ObjectPool m_observationsPool;
        public GameObject observationPrefab;
        public FastPointCloud fastPointCloud;
        public FastLinesCloud pointsLinesCloud;
        public FastLinesCloud observationsLinesCloud;
        public FastLinesCloud linesCloud;

        public IConnectionsContainer<SlamLine> LinesContainer { get; private set; }
        public IConnectionsContainer<SlamLine> ObservationsConnections { get; private set; }
        public IConnectionsContainer<SlamLine> PointsConnections { get; private set; }
        public ICloudObjectsContainer<SlamPoint> PointsContainer { get; private set; }
        public GameObjectsContainer<TrackedObjPb> TrackedObjsContainer { get; private set; }
        public GameObjectsContainer<SlamObservation> ObservationsContainer { get; private set; }

        private void Awake()
        {
            var invoker = FindObjectOfType<MainThreadInvoker>();
            TrackedObjsContainer = new TrackedObjectsContainer(helmetPrefab, invoker);

            LinesContainer = new SlamLinesContainer(linesCloud);
            
            var observationsContainer = new SlamObservationsContainer(observationPrefab, invoker);
            ObservationsContainer = observationsContainer;
            m_observationsPool = observationsContainer.ObservationsPool;

            ObservationsConnections = new SlamLinesContainer(observationsLinesCloud);

            PointsContainer = new SlamPointsContainer(fastPointCloud);
            PointsConnections = new SlamLinesContainer(pointsLinesCloud);
        }

        public void SetActivePointCloud(bool value)
        {
            fastPointCloud.SetActive(value);
            pointsLinesCloud.SetActive(value);
        }
        public void SetActiveObservationsGraph(bool value)
        {
            m_observationsPool.SetActive(value);
            observationsLinesCloud.SetActive(value);
        }

        public void SetActiveLinesCloud(bool value) => linesCloud.SetActive(value);

        public void Clear()
        {
            TrackedObjsContainer.Clear();
            ObservationsContainer.Clear();
            LinesContainer.Clear();
            PointsContainer.Clear();
            ObservationsConnections.Clear();
            PointsConnections.Clear();
        }
    }
}
