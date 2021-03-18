using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using RosSharp.RosBridgeClient;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public abstract class MessageHandler<TMessage, TCloudItem> : IMessageHandler
            where TMessage : Message
            where TCloudItem : ICloudItem
    {
        protected IContainer<TCloudItem>? Container;
        private RosSocket? _socket;
        private readonly string _subscriptionId;

        protected MessageHandler(IContainerTree container, RosSocket socket, string topic)
        {
            _socket = socket;
            Container = (IContainer<TCloudItem>) container;
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