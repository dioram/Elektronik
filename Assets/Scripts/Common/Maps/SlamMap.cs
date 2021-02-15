using Elektronik.Common.Clouds;
using Elektronik.Common.Clouds.V2;
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

        public GameObject observationPrefab;
        public FastPointCloudV2 fastPointCloud;
        public FastLineCloudV2 pointsConnectionsCloud;
        public FastLineCloudV2 observationsConnectionsCloud;
        public FastLineCloudV2 trackedObjsConnectionsCloud;
        public FastLineCloudV2 linesCloud;
        public FastPlaneCloudV2 planeCloud;

        public IConnectableObjectsContainer<SlamPoint> Points { get; private set; }
        public GameObjectsContainer<SlamObservation> ObservationsGO { get; private set; }
        public GameObjectsContainer<SlamTrackedObject> TrackedObjsGO { get; private set; }

        public IConnectableObjectsContainer<SlamObservation> Observations { get; private set; }
        public IConnectableObjectsContainer<SlamTrackedObject> TrackedObjs { get; private set; }
        
        public SlamInfinitePlanesContainer InfinitePlanes { get; private set; }

        public ILinesContainer<SlamLine> Lines { get; private set; }

        private void Awake()
        {
            var trackedObjs = new TrackedObjectsContainer(helmetPrefab);
            TrackedObjs = new ConnectableObjectsContainer<SlamTrackedObject>(
                trackedObjs,
                new SlamLinesContainer(trackedObjsConnectionsCloud));
            TrackedObjsGO = trackedObjs;

            var observations = new SlamObservationsContainer(observationPrefab);
            Observations = new ConnectableObjectsContainer<SlamObservation>(
                observations,
                new SlamLinesContainer(observationsConnectionsCloud));
            ObservationsGO = observations;

            Points = new ConnectableObjectsContainer<SlamPoint>(
                new SlamPointsContainer(fastPointCloud),
                new SlamLinesContainer(pointsConnectionsCloud));

            Lines = new SlamLinesContainer(linesCloud);
            InfinitePlanes = new SlamInfinitePlanesContainer(planeCloud);
        }

        public void SetActivePointCloud(bool value)
        {
            fastPointCloud.SetActive(value);
            pointsConnectionsCloud.SetActive(value);
        }
        public void SetActiveObservationsGraph(bool value)
        {
            ObservationsGO.ObservationsPool.SetActive(value);
            observationsConnectionsCloud.SetActive(value);
        }

        public void SetActivePlaneCloud(bool value)
        {
            planeCloud.SetActive(value);
        }

        public void Clear()
        {
            TrackedObjs.Clear();
            Observations.Clear();
            Points.Clear();
        }
    }
}
