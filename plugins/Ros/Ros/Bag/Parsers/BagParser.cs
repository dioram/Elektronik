using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Ros.Bag.Parsers.Records;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers
{
    public class BagParser : IDisposable
    {
        private readonly FileStream _file;
        private (int, int)? _version;
        private List<Connection>? _connections;

        public BagParser(string fileName)
        {
            _file = File.Open(fileName, FileMode.Open);
            if (Version.major != 2 || Version.minor != 0) throw new ParsingException("Only version 2.0 is supported.");
        }

        public (int major, int minor) Version => _version ?? ReadVersion();

        public IEnumerable<Record> ReadAll()
        {
            _file.Position = 13;
            while (_file.Position < _file.Length)
            {
                var record = RecordsFactory.Read(_file);
                if (record == null) continue;
                yield return record;
            }
        }

        public IEnumerable<Connection> GetTopics()
        {
            _connections ??= new List<Connection>();
            _file.Position = 13;
            while (_file.Position < _file.Length)
            {
                var record = RecordsFactory.Read(_file) as Connection;
                if (record == null) continue;
                _connections.Add(record);
                yield return record;
            }
        }

        public IEnumerable<MessageData> ReadMessages()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            if (_connections == null) GetTopics().ToList();
            _file.Position = 13;
            while (_file.Position < _file.Length)
            {
                var record = RecordsFactory.Read(_file);
                if (record is not Chunk chunk) continue;
                var messages = chunk.Unchunk();
                foreach (var message in messages)
                {
                    message.SetTopic(_connections!);
                    yield return message;
                }
            }
        }

        public void Dispose()
        {
            _file.Dispose();
        }

        #region Private

        private (int, int) ReadVersion()
        {
            _file.Position = 0;
            var str = Encoding.UTF8.GetString(_file.ReadBytes(13));
            if (!str.StartsWith("#ROSBAG V")) throw new ParsingException("This is not rosbag file.");
            _version = (int.Parse(str[9].ToString()), int.Parse(str[11].ToString()));
            return _version.Value;
        }

        #endregion
    }
}