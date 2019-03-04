using Elektronik.Common;
using System.Linq;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class SlamEventCommand : MacroCommand
    {
        public SlamEventCommand(
            ISlamContainer<SlamPoint> pointsContainer,
            ISlamContainer<SlamLine> linesContainer,
            SlamObservationsGraph graph,
            Helmet helmet,
            Package slamEvent)
        {
            m_commands.Add(new AddCommand(pointsContainer, linesContainer, graph, slamEvent));
            m_commands.Add(new UpdateCommand(pointsContainer, graph, helmet, slamEvent));
        }
    }
}
