using System;

namespace Elektronik.DataSources.SpecialInterfaces
{
    // TODO: May be rename somehow. All data sources are visible, this interface means that it can be hidden, but I don't like name "Hideable".
    
    /// <summary> Marks that data source can be hidden. </summary>
    public interface IVisibleDataSource : IDataSource
    {
        /// <summary> Is data source is visible now? </summary>
        public bool IsVisible { get; set; }

        /// <summary> This event will be raised when visibility changed. </summary>
        public event Action<bool> OnVisibleChanged;
    }
}