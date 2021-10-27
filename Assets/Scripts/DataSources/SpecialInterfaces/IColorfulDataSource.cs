using UnityEngine;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that cloud container has its own color. </summary>
    public interface IColorfulDataSource : IDataSource
    {
        /// <summary> Color of this container. </summary>
        Color Color { get; }
    }
}