using System;
using System.IO;
using UnityEngine;

namespace Elektronik.Loggers
{
    public static class ElektronikLogger
    {
        private static StreamWriter _logger;

        public static void OpenLog()
        {
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            directory = directory.Parent;
            string directoryPath = Path.Combine(directory.FullName, "Logs");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            string filename = $"log_{DateTime.Now:MM_dd_yyyy hh_mm_ss}.txt";
            string logFileName = Path.Combine(directoryPath, filename);
            _logger = new StreamWriter(logFileName);
        }

        public static void CloseLog()
        {
            _logger.Dispose();
        }

        public static void Log(string logString, string stackTrace, LogType type)
        {
#if DEBUG
            _logger.WriteLine("{0}: {1}", type.ToString().ToUpper(), logString);
            if (type == LogType.Exception)
            {
                _logger.WriteLine(stackTrace);
            }
            _logger.WriteLine();
            _logger.WriteLine(new String('/', 50));
            _logger.WriteLine();
#endif
        }

        public static void WrapDebug(Action action)
        {
#if DEBUG
            action();
#endif
        }
    }
}
