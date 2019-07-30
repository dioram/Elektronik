namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public interface IPackageViewUpdateCommand
    {
        void Execute();
        void UnExecute();
    }
}
