using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class UpdateCommand : ISlamEventCommand
    {
        private readonly SlamObservation[] m_observations2Restore;
        private readonly SlamObservation[] m_observations2Update;

        private readonly SlamPoint[] m_points2Restore;
        private readonly SlamPoint[] m_points2Update;

        private readonly ICloudObjectsContainer<SlamPoint> m_pointsContainer;
        private readonly ICloudObjectsContainer<SlamObservation> m_observationsContainer;
        private readonly Helmet m_helmet;
        private readonly SlamObservation m_helmetPose;


        public UpdateCommand(
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamObservation> graph,
            Helmet helmet,
            SlamPackage slamEvent) : this(pointsContainer, graph, helmet, slamEvent.Points, slamEvent.Observations)
        {}

        public UpdateCommand(
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamObservation> observationsContainer,
            Helmet helmet,
            IEnumerable<SlamPoint> points,
            IEnumerable<SlamObservation> observations)
        {
            m_pointsContainer = pointsContainer;
            m_observationsContainer = observationsContainer;
            m_helmet = helmet;

            if (points != null)
            {
                m_points2Restore = points.Where(p => p.id != -1).Select(p => pointsContainer[p]).ToArray();
                m_points2Update = points.Where(p => p.id != -1).ToArray();
            }

            if (observations != null)
            {
                m_helmetPose = observations.FirstOrDefault(o => o.Point.id == -1);
                m_observations2Restore = observations
                    .Where(o => o.Point.id != -1)
                    .Where(o => !o.Point.isRemoved)
                    .Select(o => observationsContainer[o])
                    .ToArray();
                m_observations2Update = observations
                    .Where(o => o.Point.id != -1)
                    .Where(o => !o.Point.isRemoved)
                    .ToArray();
            }
        }

        public void Execute()
        {
            if (m_points2Update != null)
            {
                foreach (var point in m_points2Update)
                {
                    if (point.justColored)
                    {
                        m_pointsContainer.ChangeColor(point); // не требуется менять положение
                    }
                    else
                    {
                        m_pointsContainer.Update(point); // требуется сменить цвет/положение
                    }
                }
            }

            if (m_helmetPose != null)
            {
                m_helmet.ReplaceAbs(m_helmetPose.Point.position, m_helmetPose.Orientation);
            }

            if (m_observations2Update != null)
            {
                foreach (var observation in m_observations2Update)
                {
                    m_observationsContainer.Update(observation);
                }
            }
        }

        public void UnExecute()
        {
            if (m_points2Restore != null)
            {
                foreach (var point in m_points2Restore)
                {
                    m_pointsContainer.Update(point); // восстанавливаем и положение и цвет
                }
            }

            if (m_helmetPose != null)
            {
                m_helmet.TurnBack();
            }

            if (m_observations2Restore != null)
            {
                foreach (var observation in m_observations2Restore)
                {
                    m_observationsContainer.Update(observation);
                }
            }
        }
    }
}
