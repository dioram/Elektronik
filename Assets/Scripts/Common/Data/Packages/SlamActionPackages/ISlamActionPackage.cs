namespace Elektronik.Common.Data.Packages
{
    public interface ISlamActionPackage : IPackage
    {
        ObjectType ObjectType { get; }
        ActionType ActionType { get; }
    }
}
