using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class PostProcessingCommand : MacroCommand
    {
        public PostProcessingCommand(
            ISlamContainer<SlamPoint> pointsContainer,
            ISlamContainer<SlamLine> linesContainer,
            ISlamContainer<SlamObservation> graph,
            Helmet helmet,
            Package slamEvent)
        {
            IEnumerable<SlamPoint> points = null;
            if (slamEvent.Points != null)
            {
                //points = slamEvent.Points.Where(p => p.id != -1).ToArray();
                points = slamEvent.Points
                    .Where(p => p.id != -1)
                    .Select(pointsContainer.Get)
                    .Select(p => { p.color = p.defaultColor; return p; });
                m_commands.Add(new UpdateCommand(pointsContainer, graph, helmet, points, null));
                m_commands.Add(new RemoveCommand(pointsContainer, linesContainer, graph, slamEvent));
            }
            
        }
    }
}
