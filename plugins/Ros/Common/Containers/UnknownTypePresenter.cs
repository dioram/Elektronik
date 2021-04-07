using System;
using System.Linq;
using System.Reflection;
using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class UnknownTypePresenter : PresenterBase<Message, StringRenderer, string>
    {
        public UnknownTypePresenter(string displayName) : base(displayName)
        {
        }

        protected override string ToRenderType(Message message)
        {
            return message.GetData();
        }

        public static bool CanParseTopic(string topic)
        {
            return SupportedTypes
                            .FirstOrDefault(t => (string) t.GetField("RosMessageName")?.GetValue(null) == topic)
                    != null;
        }

        private static readonly Type[] SupportedTypes = Assembly.GetAssembly(typeof(Message))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Message)))
                .ToArray();
    }
}