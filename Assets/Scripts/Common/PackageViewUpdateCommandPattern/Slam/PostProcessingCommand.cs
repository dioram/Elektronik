using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class PostProcessingCommand<T> : MacroCommand
    {
        public PostProcessingCommand(IContainer<T> container, IEnumerable<T> objects)
        {
            if (objects != null)
            {
                m_commands.Add(new UpdateCommand<T>(container, objects));
                m_commands.Add(new RemoveCommand<T>(container, objects));
            }
        }
    }
}
