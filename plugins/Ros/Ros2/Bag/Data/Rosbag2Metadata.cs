using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace Elektronik.RosPlugin.Ros2.Bag.Data
{
    public class Rosbag2Metadata
    {
        public static Rosbag2Metadata ReadFromFile(string path)
        {
            var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .Build();
            var yamlData = File.ReadAllText(path);
            return deserializer.Deserialize<Rosbag2Metadata>(yamlData);
        }

        public string[] DBPaths(string root)
        {
            return rosbag2_bagfile_information.relative_file_paths
                    .Select(path => Path.Combine(root, path))
                    .ToArray();
        }
        
        public Rosbag2BagfileInformation rosbag2_bagfile_information { get; set; }
    }
    public class Rosbag2BagfileInformation
    {
        public string[] relative_file_paths { get; set; }
    }
}