using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using RosSharp.RosBridgeClient;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public abstract class MessageHandler<TMessage, TContainer> : IMessageHandler
            where TMessage : Message
            where TContainer : class
    {
        protected TContainer? Container;
        private RosSocket? _socket;
        private readonly string _subscriptionId;

        protected MessageHandler(ISourceTree container, RosSocket socket, string topic)
        {
            _socket = socket;
            Container = (TContainer) container;
            _subscriptionId = _socket.Subscribe(topic, new SubscriptionHandler<TMessage>(Handle));
        }

        protected abstract void Handle(TMessage message);

        public void Dispose()
        {
            Container = null;
            _socket?.Unsubscribe(_subscriptionId);
            _socket = null;
        }
    }
}