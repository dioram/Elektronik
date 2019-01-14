using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline
{
    [Serializable]
    public class RecentFile
    {
        public string Path { get; private set; }
        public DateTime Time { get; private set; }
        public FileSettings Settings { get; private set; }

        public RecentFile(string path)
        {
            Path = path;
        }

        public void Update(string path)
        {
            Update(path, Settings);
        }

        public void Update(FileSettings settings)
        {
            Update(Path, settings);
        }

        public void Update(string path, FileSettings settings)
        {
            Path = path;
            Time = DateTime.Now;
            Settings = settings;
        }
    }
}
