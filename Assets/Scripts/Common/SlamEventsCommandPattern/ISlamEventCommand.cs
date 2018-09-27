using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public interface ISlamEventCommand
    {
        void Execute();
        void UnExecute();
    }
}
