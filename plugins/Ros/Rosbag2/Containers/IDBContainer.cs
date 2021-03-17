using Elektronik.Ros.Rosbag2.Data;
using SQLite;

namespace Elektronik.Ros.Rosbag2.Containers
{
    public interface IDBContainer
    {
        public long Timestamp { get; }
        
        public SQLiteConnection DBModel { get; set; }
        
        public Topic Topic { get; set; }
        
        public long[] ActualTimestamps { get; set; }

        public void ShowAt(long newTimestamp, bool rewind = false);
    }
}