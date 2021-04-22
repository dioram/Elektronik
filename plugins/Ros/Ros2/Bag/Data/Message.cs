using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Data
{
    [Table("messages")]
    public class Message
    {
        [Column("id")]
        public int ID { get; set; }

        [Column("topic_id")]
        public int TopicID { get; set; }

        [Column("timestamp")]
        public long Timestamp { get; set; }

        [Column("data")]
        
#pragma warning disable 8618
        public byte[] Data { get; set; }
#pragma warning restore 8618
    }
}
