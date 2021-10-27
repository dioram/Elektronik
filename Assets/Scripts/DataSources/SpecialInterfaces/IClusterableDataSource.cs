using Elektronik.DataObjects;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that cloud container can be divided to several clusters. </summary>
    public interface IClusterableDataSource : IDataSource
    {
        /// <summary> Get all content of cloud at this moment as points. </summary>
        public SlamPoint[] GetAllPoints();
    }
}