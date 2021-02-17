using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public class ConnectableTrackedObjsContainer : ConnectableObjectsContainer<SlamTrackedObject>
    {
        public TrackedObjectsContainer TrackedObjsContainer;
        public SlamLinesContainer LinesContainer;

        #region Unity events

        private void Awake()
        {
            Connects = LinesContainer;
            Objects = TrackedObjsContainer;
        }

        #endregion
    }
}