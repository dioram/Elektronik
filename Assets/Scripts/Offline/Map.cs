using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Offline
{
    public class Map : MonoBehaviour
    {
        public GameObject observationPrefab;
        public FastPointCloud fastPointCloud;
        public FastLinesCloud fusionLinesCloud;
        public FastLinesCloud graphConnectionLinesCloud;

        public ICloudObjectsContainer<SlamObservation> ObservationsContainer { get; private set; }
        public ICloudObjectsContainer<SlamLine> LinesContainer { get; private set; }
        public ICloudObjectsContainer<SlamPoint> PointsContainer { get; private set; }

        private void Awake()
        {
            LinesContainer = new SlamLinesContainer(fusionLinesCloud);
            ObservationsContainer = new SlamObservationsContainer(
                observationPrefab,
                new SlamLinesContainer(graphConnectionLinesCloud));
            PointsContainer = new SlamPointsContainer(fastPointCloud);
        }
    }
}
