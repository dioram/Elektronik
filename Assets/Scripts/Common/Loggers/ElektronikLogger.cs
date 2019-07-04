using System;
using System.IO;
using UnityEngine;

namespace Elektronik.Common.Loggers
{
    public static class ElektronikLogger
    {
        private static StreamWriter m_logger;

        public static void OpenLog()
        {
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            directory = directory.Parent;
            string directoryPath = Path.Combine(directory.FullName, "Logs");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            string filename = string.Format("log_{0}.txt", DateTime.Now.ToString(@"MM_dd_yyyy hh_mm_ss"));
            string logFileName = Path.Combine(directoryPath, filename);
            m_logger = new StreamWriter(logFileName);
        }

        public static void CloseLog()
        {
            m_logger.Dispose();
        }

        public static void Log(string logString, string stackTrace, LogType type)
        {
#if DEBUG
            m_logger.WriteLine("{0}: {1}", type.ToString().ToUpper(), logString);
            if (type == LogType.Exception)
            {
                m_logger.WriteLine(stackTrace);
            }
            m_logger.WriteLine();
            m_logger.WriteLine(new String('/', 50));
            m_logger.WriteLine();
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
