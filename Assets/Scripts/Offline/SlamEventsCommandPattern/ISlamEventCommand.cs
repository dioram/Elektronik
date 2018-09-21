using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public interface ISlamEventCommand
    {
        void Execute();
        void UnExecute();
    }
}
