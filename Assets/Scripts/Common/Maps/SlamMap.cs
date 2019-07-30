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
        public FastLinesCloud fusionLinesCloud;
        public FastLinesCloud graphConnectionLinesCloud;

        private SlamObservationsContainer m_observationsContainer;
        public ICloudObjectsContainer<SlamObservation> ObservationsContainer { get => m_observationsContainer; }
        public ICloudObjectsContainer<SlamLine> LinesContainer { get; private set; }
        public ICloudObjectsContainer<SlamPoint> PointsContainer { get; private set; }

        private void Awake()
        {
            LinesContainer = new SlamLinesContainer(fusionLinesCloud);
            m_observationsContainer = new SlamObservationsContainer(
                observationPrefab,
                new SlamLinesContainer(graphConnectionLinesCloud));
            PointsContainer = new SlamPointsContainer(fastPointCloud);
        }

        public void SetActivePointCloud(bool value)
        {
            fastPointCloud.SetActive(value);
            fusionLinesCloud.SetActive(value);
        }
        public void SetActiveObservationsGraph(bool value)
        {
            var obsGraphPool = m_observationsContainer.ObservationsPool;
            obsGraphPool.SetActive(value);
            graphConnectionLinesCloud.SetActive(value);
        }

        public override void Repaint()
        {
            ObservationsContainer.Repaint();
            LinesContainer.Repaint();
            PointsContainer.Repaint();
        }

        public override void Clear()
        {
            PointsContainer.Clear();
            LinesContainer.Clear();
            ObservationsContainer.Clear();
        }
    }
}
