using Elektronik.Common;
using Elektronik.Common.Events;
using System.Linq;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class SlamEventCommand : MacroCommand
    {
        public SlamEventCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph, ISlamEvent slamEvent)
        {
            m_commands.Add(new AddCommand(pointsContainer, linesContainer, graph, slamEvent));
            m_commands.Add(new ChangeColorCommand(pointsContainer, slamEvent.Points.Where(p => p.id != -1).ToArray()));
        }
    }
}
