using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public class ConnectableObservationContainer : ConnectableObjectsContainer<SlamObservation>
    {
        public SlamObservationsContainer ObservationsContainer;
        public SlamLinesContainer ConnectionsContainer;

        #region Unity events

        private void Awake()
        {
            Connects = ConnectionsContainer;
            Objects = ObservationsContainer;
        }

        #endregion
    }
}