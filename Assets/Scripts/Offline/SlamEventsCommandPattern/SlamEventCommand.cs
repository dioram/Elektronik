using Elektronik.Common;
using Elektronik.Offline.Events;
using System.Linq;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public class SlamEventCommand : MacroCommand
    {
        public SlamEventCommand(FastPointCloud pointCloud, FastLinesCloud linesCloud, SlamObservationsGraph graph, ISlamEvent operand)
        {
            if (operand.Points != null)
                m_commands.Add(new RepaintPointsCommand(pointCloud, operand.Points.Where(p => p.id != -1).ToArray()));
            if (operand.Observations != null)
                m_commands.Add(new RepaintObservationsCommand(graph, operand.Observations));
            if (operand.Lines != null)
                m_commands.Add(new RepaintLinesCommand(linesCloud, pointCloud, operand.Lines));
        }
    }
}
