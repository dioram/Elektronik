﻿namespace Elektronik.Containers.SpecialInterfaces
{
    public interface ITraceable
    {
        public bool TraceEnabled { get; set; }
        public int TraceDuration { get; set; }
    }
}