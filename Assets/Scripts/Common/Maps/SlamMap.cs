using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Extensions;
using UnityEngine;

namespace Elektronik.Common.Maps
{
    public class SlamMap : RepaintableObject
    {
        public GameObject observationPrefab;
        public FastPointCloud fastPointCloud;
        public FastLinesCloud pointsLinesCloud;
        public FastLinesCloud observationsLinesCloud;
        public FastLinesCloud linesCloud;

        private SlamObservationsContainer m_observationsContainer;
        public IConnectionsContainer<SlamLine> LinesContainer { get; private set; }
        public ICloudObjectsContainer<SlamObservation> ObservationsContainer { get => m_observationsContainer; }
        public IConnectionsContainer<SlamLine> ObservationsConnections { get; private set; }
        public ICloudObjectsContainer<SlamPoint> PointsContainer { get; private set; }
        public IConnectionsContainer<SlamLine> PointsConnections { get; private set; }

        private void Awake()
        {
            LinesContainer = new SlamLinesContainer(linesCloud);
            
            m_observationsContainer = new SlamObservationsContainer(observationPrefab);
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
            var obsGraphPool = m_observationsContainer.ObservationsPool;
            obsGraphPool.SetActive(value);
            observationsLinesCloud.SetActive(value);
        }

        public void SetActiveLinesCloud(bool value)
        {
            linesCloud.SetActive(value);
        }

        public override void Repaint()
        {
            ObservationsContainer.Repaint();
            LinesContainer.Repaint();
            PointsContainer.Repaint();
            ObservationsConnections.Repaint();
            PointsConnections.Repaint();
        }

        public override void Clear()
        {
            PointsContainer.Clear();
            PointsConnections.Clear();
            LinesContainer.Clear();
            ObservationsContainer.Clear();
            ObservationsConnections.Clear();
        }
    }
}
