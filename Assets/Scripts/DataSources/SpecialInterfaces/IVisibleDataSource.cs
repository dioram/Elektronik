using System;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that data source can be hidden. </summary>
    public interface IVisibleDataSource : IDataSource
    {
        /// <summary> Is data source is visible now? </summary>
        public bool IsVisible { get; set; }

        /// <summary> This event will be raised when visibility changed. </summary>
        public event Action<bool> OnVisibleChanged;
    }
}