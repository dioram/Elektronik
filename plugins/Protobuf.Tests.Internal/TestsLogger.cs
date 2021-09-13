using System;
using Grpc.Core.Logging;
using NUnit.Framework;

namespace Protobuf.Tests.Internal
{
    internal class TestsLogger : ILogger
    {
        public void Debug(string message) => TestContext.WriteLine($"LOGGER_Debug: {message}");

        public void Debug(string format, params object[] formatArgs) => TestContext.WriteLine($"LOGGER_Debug: {format}", formatArgs);

        public void Error(string message) => TestContext.WriteLine($"LOGGER_Error: {message}");

        public void Error(string format, params object[] formatArgs) => TestContext.WriteLine($"LOGGER_Error: {format}", formatArgs);

        public void Error(Exception exception, string message) 
            => TestContext.WriteLine($"LOGGER_Error: {message}.\n{exception.Message}\n{exception.StackTrace}");

        public ILogger ForType<T>() => this;

        public void Info(string message) => TestContext.WriteLine($"LOGGER_Info: {message}");

        public void Info(string format, params object[] formatArgs) => TestContext.WriteLine($"LOGGER_Info: {format}", formatArgs);

        public void Warning(string message) => TestContext.WriteLine($"LOGGER_Warning: {message}");

        public void Warning(string format, params object[] formatArgs) => TestContext.WriteLine($"LOGGER_Warning: {format}", formatArgs);

        public void Warning(Exception exception, string message) 
            => TestContext.WriteLine($"LOGGER_Warning: {message}.\n{exception.Message}\n{exception.StackTrace}");
    }
}