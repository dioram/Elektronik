namespace Elektronik.Common.SlamEventsCommandPattern
{
    public interface ISlamEventCommand
    {
        void Execute();
        void UnExecute();
    }
}
