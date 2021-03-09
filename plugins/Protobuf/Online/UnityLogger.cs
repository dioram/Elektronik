using System;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online
{
    internal class UnityLogger : ILogger
    {
        public void Debug(string message) => UnityEngine.Debug.Log(message);

        public void Debug(string format, params object[] formatArgs) => UnityEngine.Debug.LogFormat(format, formatArgs);

        public void Error(string message) => UnityEngine.Debug.LogError(message);

        public void Error(string format, params object[] formatArgs) => UnityEngine.Debug.LogErrorFormat(format, formatArgs);

        public void Error(Exception exception, string message) => UnityEngine.Debug.LogError(message);

        public ILogger ForType<T>() => this;

        public void Info(string message) => UnityEngine.Debug.Log(message);

        public void Info(string format, params object[] formatArgs) => UnityEngine.Debug.LogFormat(format, formatArgs);

        public void Warning(string message) => UnityEngine.Debug.LogWarning(message);

        public void Warning(string format, params object[] formatArgs) => UnityEngine.Debug.LogWarningFormat(format, formatArgs);

        public void Warning(Exception exception, string message)
        {
            UnityEngine.Debug.LogWarning(message);
            throw exception;
        }
    }
}