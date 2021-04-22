using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    internal class UnityLogger : Logger
    {
        public override void Error(string message) => Debug.LogError(message);

        public override void Info(string message) => Debug.Log(message);
    }
}