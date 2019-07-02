﻿using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class PostProcessingCommand : MacroCommand
    {
        public PostProcessingCommand(
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamLine> linesContainer,
            ICloudObjectsContainer<SlamObservation> observationsContainer,
            SlamPackage slamEvent)
        {
            IEnumerable<SlamPoint> points = null;
            if (slamEvent.Points != null)
            {
                points = slamEvent.Points
                    .Where(p => p.id != -1)
                    .Select(p => pointsContainer[p])
                    .Select(p => { p.color = p.defaultColor; return p; });
                m_commands.Add(new UpdateCommand(pointsContainer, observationsContainer, points, null));
                m_commands.Add(new RemoveCommand(pointsContainer, linesContainer, observationsContainer, slamEvent));
            }
        }
    }
}