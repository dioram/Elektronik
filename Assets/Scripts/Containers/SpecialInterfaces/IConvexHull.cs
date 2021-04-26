using System;
using System.Collections.Generic;
using Elektronik.Clouds;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface IConvexHull
    {
        List<ConvexMesh> Hulls { get; set; }
        
        bool HullVisible { get; set; }
        
        event Action<bool> OnHullVisibleChanged;
    }
}