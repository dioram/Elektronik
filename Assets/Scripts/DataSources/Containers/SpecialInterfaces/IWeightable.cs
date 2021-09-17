using System;

namespace Elektronik.DataSources.Containers.SpecialInterfaces
{
    public interface IWeightable
    {
        int MaxWeight { get; }
        event Action<int> OnMaxWeightChanged; 
        
        int MinWeight { get; set; }
        
    }
}