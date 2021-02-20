using System;
using UnityEngine;

namespace Elektronik.Online
{
    using UnityDebug = Debug;
    using IGrpcLogger = Grpc.Core.Logging.ILogger;

    public partial class Server
    {
        private class UnityLogger : IGrpcLogger
        {
            public void Debug(string message) => UnityDebug.Log(message);

            public void Debug(string format, params object[] formatArgs) => UnityDebug.LogFormat(format, formatArgs);

            public void Error(string message) => UnityDebug.LogError(message);

            public void Error(string format, params object[] formatArgs) => UnityDebug.LogErrorFormat(format, formatArgs);

            public void Error(Exception exception, string message) => UnityDebug.LogError(message);

            public IGrpcLogger ForType<T>() => this;

            public void Info(string message) => UnityDebug.Log(message);

            public void Info(string format, params object[] formatArgs) => UnityDebug.LogFormat(format, formatArgs);

            public void Warning(string message) => UnityDebug.LogWarning(message);

            public void Warning(string format, params object[] formatArgs) => UnityDebug.LogWarningFormat(format, formatArgs);

            public void Warning(Exception exception, string message)
            {
                UnityDebug.LogWarning(message);
                throw exception;
            }
        }
    }
}
