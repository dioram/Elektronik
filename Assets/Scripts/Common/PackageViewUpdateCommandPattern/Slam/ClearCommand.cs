using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class ClearCommand : IPackageViewUpdateCommand
    {
        private readonly ICloudObjectsContainer<SlamPoint> m_pointsContainer;
        private readonly ICloudObjectsContainer<SlamLine> m_linesContainer;
        private readonly ICloudObjectsContainer<SlamObservation> m_observationsContainer;

        private readonly SlamLine[] m_undoLines;
        private readonly SlamPoint[] m_undoPoints;
        private readonly SlamObservation[] m_undoObservations;

        public ClearCommand(
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamLine> linesContainer,
            ICloudObjectsContainer<SlamObservation> observationsContainer)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_observationsContainer = observationsContainer;

            m_undoLines = m_linesContainer.GetAll();
            m_undoPoints = m_pointsContainer.GetAll();
            m_undoObservations = m_observationsContainer.GetAll();
        }

        public void Execute()
        {
            Debug.Log("[Clear Execute]");
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            m_observationsContainer.Clear();
        }

        public void UnExecute()
        {
            Debug.Log("[Clear UnExecute]");
            m_pointsContainer.AddRange(m_undoPoints);
            m_linesContainer.AddRange(m_undoLines);
            m_observationsContainer.AddRange(m_undoObservations);
        }
    }
}
