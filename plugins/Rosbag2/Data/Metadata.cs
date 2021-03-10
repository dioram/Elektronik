using System;
using System.IO;
using YamlDotNet.Serialization;

// ReSharper disable InconsistentNaming

namespace Elektronik.Rosbag2.Data
{
    [Serializable]
    public class Metadata
    {
        public static Metadata ReadFromFile(string file)
        {
            var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .Build();
            return deserializer.Deserialize<Metadata>(File.ReadAllText(file));
        }
            
        [Serializable]
        public class BagFileInfo
        {
            public int version;
            public string storage_identifier;
            public string[] relative_file_paths;
            public int message_count;
            public StartingTime starting_time;

            [Serializable]
            public class StartingTime
            {
                public ulong nanoseconds_since_epoch;
            }
        }

        public BagFileInfo rosbag2_bagfile_information;
    }
}