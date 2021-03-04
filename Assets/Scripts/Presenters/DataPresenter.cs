namespace Elektronik.Presenters
{
    public abstract class DataPresenter : IChainable<DataPresenter>
    {
        protected DataPresenter Successor;

        public IChainable<DataPresenter> SetSuccessor(IChainable<DataPresenter> presenter)
        {
            return Successor = presenter as DataPresenter;
        }

        public virtual void Present(object data)
        {
            Successor?.Present(data);
        }

        public virtual void SetRenderer(object dataRenderer)
        {
            Successor?.SetRenderer(dataRenderer);
        }

        public virtual void Clear()
        {
            Successor?.Clear();
        }
    }
}