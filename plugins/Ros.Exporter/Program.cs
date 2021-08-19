using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.RosPlugin.Ros.Bag.Parsers.Records;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using SQLite;
using UnityEngine;
using Color = System.Drawing.Color;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosImage = RosSharp.RosBridgeClient.MessageTypes.Sensor.Image;

namespace Ros.Exporter
{
    class Program
    {
        static Color ReadColor(Stream stream, TextureFormat format)
        {
            switch (format)
            {
            case TextureFormat.Alpha8:
            {
                var a = stream.ReadByte();
                return Color.FromArgb(255, a, a, a);
            }
            case TextureFormat.RGB24:
                return Color.FromArgb(255, stream.ReadByte(), stream.ReadByte(), stream.ReadByte());
            case TextureFormat.RGBA32:
            {
                var r = stream.ReadByte();
                var g = stream.ReadByte();
                var b = stream.ReadByte();
                var a = stream.ReadByte();
                return Color.FromArgb(a, r, g, b);
            }
            case TextureFormat.R16:
            {
                var r = (stream.ReadByte() << 8) + stream.ReadByte();
                return Color.FromArgb(255, r, 0, 0);
            }
            case TextureFormat.BGRA32:
            {
                var b = stream.ReadByte();
                var g = stream.ReadByte();
                var r = stream.ReadByte();
                var a = stream.ReadByte();
                return Color.FromArgb(a, r, g, b);
            }
            case TextureFormat.RGB48:
            {
                var r = (stream.ReadByte() << 8) + stream.ReadByte();
                var g = (stream.ReadByte() << 8) + stream.ReadByte();
                var b = (stream.ReadByte() << 8) + stream.ReadByte();
                return Color.FromArgb(255, r, g, b);
            }
            case TextureFormat.RGBA64:
            {
                var r = (stream.ReadByte() << 8) + stream.ReadByte();
                var g = (stream.ReadByte() << 8) + stream.ReadByte();
                var b = (stream.ReadByte() << 8) + stream.ReadByte();
                var a = (stream.ReadByte() << 8) + stream.ReadByte();
                return Color.FromArgb(a, r, g, b);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static readonly Dictionary<string, StreamWriter> Files = new();
        private static readonly Dictionary<int, Topic> Topics = new();

        static void CreateHeader(RosMessage data, string topicName, string path)
        {
            if (data is RosImage)
            {
                Directory.CreateDirectory(Path.Join(path, topicName.Replace("/", "_")));
                Files[topicName] = null;
                return;
            }
            if (data is VideoFrame) return;
            
            var file = File.CreateText(Path.Join(path, $"{topicName.Replace("/", "_")}.csv"));
            var header = RosMessageConvertExtender.GetMessagePropertyNames(data.GetType());
            Files[topicName] = file;
            file.WriteLine(string.Join(',', header));
        }
        
        static async Task<bool> ExportData(MessageData data, string path)
        {
            var mess = MessageParser.Parse(data.Data!, data.TopicType!, false);
            if (!Files.ContainsKey(data.TopicName)) CreateHeader(mess, data.TopicName, path);
            
            if (mess is RosImage image)
            {
                var imageMessage = ImageDataExt.FromImageMessage(image);
                var im = new Bitmap(imageMessage.Width, imageMessage.Height);
                using var stream = new MemoryStream(imageMessage.Data);
                for (int j = 0; j < imageMessage.Height; j++)
                {
                    for (int i = 0; i < imageMessage.Width; i++)
                    {
                        im.SetPixel(i, j, ReadColor(stream, imageMessage.Encoding));
                    }
                }
                
                im.Save($"{data.TopicName.Replace("/", "_")}//{data.Timestamp}.png", ImageFormat.Png);
                return true;
            }
        
            if (mess is VideoFrame) return true;
        
            await Files[data.TopicName].WriteLineAsync(String.Join(',', mess.GetData()));
            return true;
        }

        
        static async Task<bool> ExportData(Message data, Topic topic, string path)
        {
            // if (topic.Type.EndsWith("Path")) return true;
            var mess = MessageParser.Parse(data.Data, topic.Type, true);
            if (mess is null) return true;
            if (!Files.ContainsKey(topic.Name)) CreateHeader(mess, topic.Name, path);
            
            if (mess is RosImage image)
            {
                var imageMessage = ImageDataExt.FromImageMessage(image);
                var im = new Bitmap(imageMessage.Width, imageMessage.Height);
                using var stream = new MemoryStream(imageMessage.Data);
                for (int j = 0; j < imageMessage.Height; j++)
                {
                    for (int i = 0; i < imageMessage.Width; i++)
                    {
                        im.SetPixel(i, j, ReadColor(stream, imageMessage.Encoding));
                    }
                }
                
                im.Save($"{topic.Name.Replace("/", "_")}//{data.Timestamp}.png", ImageFormat.Png);
                return true;
            }
            if (mess is VideoFrame) return true;

            await Files[topic.Name].WriteLineAsync(String.Join(',', mess.GetData()));
            return true;
        }

        static async Task<int> Main(string[] args)
        {
            switch (args[0])
            {
            case "-1":
            {
                var parser = new BagParser(args[0]);
                await parser.ReadMessagesAsync()
                        .Where(m => m.TopicName is not null && m.TopicType is not null)
                        .Select(m => ExportData(m, args[2])).ToArrayAsync();
                return 0;
            }
            case "-2":
            {
                var db_paths = Rosbag2Metadata.ReadFromFile(args[1]).DBPaths(Path.GetDirectoryName(args[1]));
                foreach (var db in db_paths)
                {
                    Topics.Clear();
                    using var model = new SQLiteConnection(db, SQLiteOpenFlags.ReadOnly);
                    foreach (var topic in model.Table<Topic>())
                    {
                        Topics[topic.Id] = topic;
                    }
                    var command = model.CreateCommand("SELECT * FROM messages");
                    var messages = command.ExecuteQuery<Message>();
                    foreach (var message in messages)
                    {
                        await ExportData(message, Topics[message.TopicID], args[2]);
                    }
                }
                return 0;
            }
            default:
                return 1;
            }
        }
    }
}