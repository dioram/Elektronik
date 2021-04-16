namespace Elektronik.Containers
{
    public interface ITraceable
    {
        public bool TraceEnabled { get; set; }
        public int Duration { get; set; }
    }
}