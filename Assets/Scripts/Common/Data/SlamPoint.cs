using System;

namespace Elektronik.Common.Data
{
    public class SlamPoint : ISlamObject, IClonable<SlamPoint>
    {
        public SlamPoint Clone()
        {
            return MemberwiseClone() as SlamPoint;
        }
    }
}
