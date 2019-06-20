using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class RemoveCommand : IPackageViewUpdateCommand
    {
        private readonly SlamLine[] m_lines2Remove;
        private readonly SlamPoint[] m_points2Remove;
        private readonly SlamObservation[] m_observations2Remove;

        private readonly ICloudObjectsContainer<SlamObservation> m_observationsContainer;
        private readonly ICloudObjectsContainer<SlamLine> m_linesContainer;
        private readonly ICloudObjectsContainer<SlamPoint> m_pointsContainer;

        public RemoveCommand(
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamLine> linesContainer,
            ICloudObjectsContainer<SlamObservation> observationsContainer,
            SlamPackage slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_observationsContainer = observationsContainer;

            if (slamEvent.Points != null)
            {
                m_points2Remove = slamEvent.Points
                    .Where(p => p.id != -1)
                    .Where(p => p.isRemoved)
                    .Select(p => m_pointsContainer[p])
                    .ToArray();
            }
            if (slamEvent.Lines != null)
            {
                m_lines2Remove = slamEvent.Lines
                    .Where(l => l.isRemoved)
                    .Select(l => m_linesContainer[l])
                    .ToArray();
            }
            if (slamEvent.Observations != null)
            {
                m_observations2Remove = slamEvent.Observations
                    .Where(o => o.Point.id != -1)
                    .Where(o => o.Point.isRemoved)
                    .Select(o => m_observationsContainer[o])
                    .ToArray();
            }
        }

        public void Execute()
        {
            Debug.Log("[RemoveCommand.Execute]");
            if (m_points2Remove != null)
            {
                foreach (var point in m_points2Remove)
                {
                    m_pointsContainer.Remove(point);
                }
            }
            if (m_lines2Remove != null)
            {
                foreach (var line in m_lines2Remove)
                {
                    m_linesContainer.Remove(line);
                }
            }
            if (m_observations2Remove != null)
            {
                foreach (var observation in m_observations2Remove)
                {
                    m_observationsContainer.Remove(observation);
                }
            }
        }

        public void UnExecute()
        {
            Debug.Log("[RemoveCommand.UnExecute]");
            if (m_points2Remove != null)
            {
                foreach (var point in m_points2Remove)
                {
                    m_pointsContainer.Add(point);
                }
            }

            if (m_lines2Remove != null)
            {
                foreach (var line in m_lines2Remove)
                {
                    m_linesContainer.Add(line);
                }
            }
            if (m_observations2Remove != null)
            {
                foreach (var observation in m_observations2Remove)
                {
                    m_observationsContainer.Add(observation);
                }
            }
        }
    }
}
