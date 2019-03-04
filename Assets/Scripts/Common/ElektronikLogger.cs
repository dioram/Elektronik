using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public static class ElektronikLogger
    {
        private static StreamWriter m_logger;

        public static void OpenLog()
        {
            string logFileName = string.Format(@"logs\log_{0}.txt", DateTime.Now.ToString(@"MM_dd_yyyy hh_mm_ss"));
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
