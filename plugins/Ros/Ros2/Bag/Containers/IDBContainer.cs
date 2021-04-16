using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
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