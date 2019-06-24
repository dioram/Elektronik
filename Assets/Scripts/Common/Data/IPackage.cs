namespace Elektronik.Common.Data
{
    public interface IPackage
    {
        PackageType Type { get; }
        int Timestamp { get; }
        bool IsKey { get; }
    }
}
