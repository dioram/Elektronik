using Elektronik.Common;
using Elektronik.Common.Events;
using System.Linq;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class SlamEventCommand : MacroCommand
    {
        public SlamEventCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph, ISlamEvent operand)
        {
            if (operand.Points != null)
                m_commands.Add(new RepaintPointsCommand(pointsContainer, operand.Points.Where(p => p.id != -1).ToArray()));
            if (operand.Observations != null)
                m_commands.Add(new RepaintObservationsCommand(graph, operand.Observations));
            if (operand.Lines != null)
                m_commands.Add(new RepaintLinesCommand(pointsContainer, linesContainer, operand.Lines));
        }
    }
}
