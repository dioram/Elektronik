namespace Elektronik.RosPlugin.Common.Containers
{
    public interface IPresenter<TMessage>
    {
        public TMessage Current { get; }
        
        public void Present(TMessage data);
    }
}