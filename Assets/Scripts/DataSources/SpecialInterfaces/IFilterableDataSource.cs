using System;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that data source can be filtered by some weight coefficient. </summary>
    public interface IFilterableDataSource : IDataSource
    {
        /// <summary> Max available weight. </summary>
        int MaxWeight { get; }
        
        /// <summary> This event will be raised when max weight changed. </summary>
        event Action<int> OnMaxWeightChanged; 
        
        /// <summary> Min weight of objects to be rendered. </summary>
        int MinWeight { get; set; }
        
    }
}