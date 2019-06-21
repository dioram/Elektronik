using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamPackageCommander : RepaintablePackageViewUpdateCommander
    {
        public GameObject observationPrefab;
        public FastPointCloud fastPointCloud;
        public FastLinesCloud fusionLinesCloud;
        public FastLinesCloud graphConnectionLinesCloud;
        public EventLogger eventsLogger;

        private ICloudObjectsContainer<SlamObservation> m_observationsContainer;
        private ICloudObjectsContainer<SlamLine> m_linesContainer;
        private ICloudObjectsContainer<SlamPoint> m_pointsContainer;

        private void Awake()
        {
            m_linesContainer = new SlamLinesContainer(fusionLinesCloud);
            m_observationsContainer = new SlamObservationsContainer(
                observationPrefab,
                new SlamLinesContainer(graphConnectionLinesCloud));
            m_pointsContainer = new SlamPointsContainer(fastPointCloud);
        }

        public override LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg)
        {
            if (pkg.Type != PackageType.SLAMPackage)
                return m_commander?.GetCommands(pkg);
            var commands = new LinkedList<IPackageViewUpdateCommand>();
            var slamPkg = pkg as SlamPackage;

            if (slamPkg.Timestamp == -1)
            {
                commands.AddLast(new ClearCommand(m_pointsContainer, m_linesContainer, m_observationsContainer));
                commands.Last.Value.Execute();
                return commands;
            }

            // При добавлении объектов в карту их в карте быть не должно.
            slamPkg.TestExistent(obj => obj.id != -1 && obj.isNew, m_pointsContainer, m_observationsContainer);
            commands.AddLast(new AddCommand(m_pointsContainer, m_linesContainer, m_observationsContainer, slamPkg));
            commands.Last.Value.Execute();

            // При любых манипуляциях с картой объекты, над которыми происходят манипуляции, должны быть в карте.
            slamPkg.TestNonExistent(obj => obj.id != -1, m_pointsContainer, m_observationsContainer);
            commands.AddLast(new UpdateCommand(m_pointsContainer, m_observationsContainer, slamPkg));
            commands.Last.Value.Execute();

            commands.AddLast(new PostProcessingCommand(m_pointsContainer, m_linesContainer, m_observationsContainer, slamPkg));
            commands.Last.Value.Execute();

            if (slamPkg.IsKey)
            {
                commands.AddLast(new LambdaCommand(
                    () => eventsLogger.UpdateInfo(slamPkg, m_pointsContainer, m_observationsContainer),
                    () => eventsLogger.RestoreInfo(m_pointsContainer, m_observationsContainer)));
            }

            return commands;
        }

        public override void Repaint()
        {
            m_observationsContainer.Repaint();
            m_linesContainer.Repaint();
            m_pointsContainer.Repaint();
        }

        public override void Clear()
        {
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            m_observationsContainer.Clear();
        }

    }
}
