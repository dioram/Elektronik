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
        static ElektronikLogger()
        {
            string logFileName = string.Format(@"logs\log_{0}.txt", DateTime.Now.ToString(@"mm_dd_yyyy hh_MM_ss"));
            m_logger = new StreamWriter(logFileName);
        }

        public static void Log(string logString, string stackTrace, LogType type)
        {
#if DEBUG
            m_logger.WriteLine("{0}: {1}", type.ToString().ToUpper(), logString);
            if (type == LogType.Exception)
            {
                m_logger.WriteLine(stackTrace);
            }
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
