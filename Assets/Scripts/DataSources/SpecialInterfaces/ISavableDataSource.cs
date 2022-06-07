namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that data source can be saved by itself. </summary>
    /// <remarks>
    /// Actually all data sources can be saved using <see cref="IDataSource.TakeSnapshot()"/>.
    /// This interface marks that there should be save button in render tree near this container.
    /// </remarks>
    public interface ISavableDataSource : IDataSource
    {
    }
}