using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Elektronik.Ros.Rosbag.Parsers.Records;

namespace Elektronik.Ros.Rosbag.Parsers
{
    public class BagParser : IDisposable
    {
        private readonly FileStream _file;
        private (int, int)? _version;

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

        public IEnumerable<(string name, string type)> GetTopics()
        {
            _file.Position = 13;
            while (_file.Position < _file.Length)
            {
                var record = RecordsFactory.Read(_file) as Connection;
                if (record == null) continue;
                yield return (record.Topic, record.Type);
            }
        }

        public IEnumerable<MessageData> ReadMessages()
        {
            _file.Position = 13;
            while (_file.Position < _file.Length)
            {
                var record = RecordsFactory.Read(_file);
                if (record is not Chunk chunk) continue;
                var messages = chunk.Unchunk();
                foreach (var message in messages)
                {
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