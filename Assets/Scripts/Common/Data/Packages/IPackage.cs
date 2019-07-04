namespace Elektronik.Common.Data.Packages
{
    public interface IPackage
    {
        PackageType Type { get; }
        int Timestamp { get; }
        bool IsKey { get; }
    }
}
