namespace Elektronik.DataSources.SpecialInterfaces
{
    // TODO: rewrite this. It should be in consumers.
    
    /// <summary> Marks that data source can leave traces when updated. </summary>
    public interface ITraceableDataSource
    {
        public bool TraceEnabled { get; set; }
        public int TraceDuration { get; set; }
    }
}