using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class RepaintEntireMapCommand : MacroCommand
    {
        public RepaintEntireMapCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph, ISlamEvent slamEvent)
        {
            m_commands.Add(new ClearCommand(pointsContainer, linesContainer, graph));
            m_commands.Add(new RepaintPointsCommand(pointsContainer, slamEvent.Points));
            m_commands.Add(new RepaintLinesCommand(pointsContainer, linesContainer, slamEvent.Lines));
            m_commands.Add(new RepaintObservationsCommand(graph, slamEvent.Observations));
        }
    }
}
