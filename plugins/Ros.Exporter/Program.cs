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
using UnityEngine;
using Color = System.Drawing.Color;
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
                return Color.FromArgb(stream.ReadByte(), Color.Black);
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

        private static Dictionary<string, StreamWriter> _files = new();

        static void CreateHeader(string topicName, MessageData data)
        {
            var first = MessageParser.Parse(data.Data!, data.TopicType!, false);
            if (first is RosImage)
            {
                Directory.CreateDirectory(data.TopicName.Replace("/", "_"));
                _files[topicName] = null;
                return;
            }
            var file = File.CreateText($"{data.TopicName.Replace("/", "_")}.csv");
            var header = RosMessageConvertExtender.GetMessagePropertyNames(first.GetType());
            file.WriteLine(string.Join(',', header));
        }
        
        static async Task<bool> ExportData(MessageData data)
        {
            if (!_files.ContainsKey(data.TopicName)) CreateHeader(data.TopicName, data);
            
            var mess = MessageParser.Parse(data.Data!, data.TopicType!, false);
            if (mess is RosImage)
            {
                var imageMessage = ImageDataExt.FromImageMessage((RosImage) mess);
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

            await _files[data.TopicName].WriteLineAsync(String.Join(',', mess.GetData()));
            return true;
        }


        static async Task<int> Main(string[] args)
        {
            var parser = new BagParser(args[0]);
            await parser.ReadMessagesAsync()
                    .Where(m => m.TopicName is not null && m.TopicType is not null)
                    .Select(ExportData).ToArrayAsync();

            return 0;
        }
    }
}