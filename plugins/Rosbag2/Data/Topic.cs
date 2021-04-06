using SQLite;

namespace Elektronik.Rosbag2.Data
{
    [Table("topics")]
    public class Topic
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("serialization_format")]
        public string SerializationFormat { get; set; }

        [Column("offered_qos_profiles")]
        public string OfferedQOSProfiles { get; set; }

        protected bool Equals(Topic other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Topic) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
