using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline
{
    class EventFileReader
    {
        public ISlamEvent NextEvent()
        {
            throw new NotImplementedException();
        }

        public ISlamEvent NextKeyEvent()
        {
            throw new NotImplementedException();
        }

        public ISlamEvent PrevKeyEvent()
        {
            throw new NotImplementedException();
        }

        public uint Position { get; set; }

        public uint Length { get; private set; }

        public TimeSpan LengthInTime { get; private set; }
    }
}
