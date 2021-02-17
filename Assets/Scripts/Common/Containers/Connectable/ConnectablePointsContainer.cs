using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public class ConnectablePointsContainer : ConnectableObjectsContainer<SlamPoint>
    {
        public SlamPointsContainer PointsContainer;
        public SlamLinesContainer LinesContainer;

        #region Unity events

        private void Awake()
        {
            Connects = LinesContainer;
            Objects = PointsContainer;
        }

        #endregion
    }
}