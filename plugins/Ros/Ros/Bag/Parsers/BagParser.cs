using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Ros.Bag.Parsers.Records;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers
{
    public class BagParser : IDisposable
    {
        private readonly FileStream _file;
        private (int, int)? _version;
        private readonly List<Connection> _connections = new();

        public BagParser(string fileName)
        {
            _file = File.Open(fileName, FileMode.Open);
            if (Version.major != 2 || Version.minor != 0) throw new ParsingException("Only version 2.0 is supported.");
        }

        public (int major, int minor) Version => _version ?? ReadVersion();

        public IEnumerable<Connection> GetTopics()
        {
            if (_connections.Count == 0)
            {
                _file.Position = 13;
                while (_file.Position < _file.Length)
                {
                    var record = RecordsFactory.Read<Connection>(_file, Connection.OpCode);
                    if (record == null) continue;
                    _connections.Add(record);
                }
            }

            return _connections;
        }

        public async IAsyncEnumerable<MessageData> ReadMessagesAsync(IEnumerable<Connection>? topics = null)
        {
            int[]? actualIds = topics?.Select(c => c.Id).ToArray();
            var actualChunks = GetActualChunks(actualIds);
            var activeTasks = new ConcurrentQueue<Task<IEnumerable<MessageData>>>();

            var _ = Task.Run(() =>
            {
                while (actualChunks.Count > 0)
                {
                    while (activeTasks.Count < 10)
                    {
                        var chunk = actualChunks.Dequeue();
                        activeTasks.Enqueue(Task.Run(() => ReadAndUnchunk(chunk, actualIds)));
                    }
                }
            });

            while (actualChunks.Count > 0 || activeTasks.Count > 0)
            {
                if (!activeTasks.TryDequeue(out var task)) continue;
                var data = await task;
                foreach (var message in data)
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

        private IEnumerable<MessageData> ReadAndUnchunk(long filePos, int[]? allowedTopicIds = null)
        {
            Chunk record;
            lock (_file)
            {
                _file.Position = filePos;
                record = RecordsFactory.Read<Chunk>(_file, Chunk.OpCode)!;
            }

            return record!.Unchunk(allowedTopicIds);
        }

        private Queue<long> GetActualChunks(int[]? actualIds = null)
        {
            Queue<long> actualChunks = new();
            _file.Position = 13;
            while (_file.Position < _file.Length)
            {
                var record = RecordsFactory.Read<ChunkInfo>(_file, ChunkInfo.OpCode);
                if (record == null) continue;
                if (actualIds == null
                    || actualIds.Any(id => record.GetIds().Contains(id) && !actualChunks.Contains(record.ChunkPos)))
                {
                    actualChunks.Enqueue(record.ChunkPos);
                }
            }

            return actualChunks;
        }

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