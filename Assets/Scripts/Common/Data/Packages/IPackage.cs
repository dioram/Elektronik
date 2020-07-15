namespace Elektronik.Common.Data.Packages
{
    public interface IPackage
    {
        PackageType PackageType { get; }
        int Timestamp { get; }
        bool IsKey { get; }
    }
}
