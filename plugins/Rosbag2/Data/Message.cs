using SQLite;

namespace Elektronik.Rosbag2.Data
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
        public byte[] Data { get; set; }
    }
}
