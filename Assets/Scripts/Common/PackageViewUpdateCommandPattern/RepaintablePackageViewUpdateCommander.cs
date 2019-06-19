namespace Elektronik.Common.PackageViewUpdateCommandPattern
{
    public abstract class RepaintablePackageViewUpdateCommander : PackageViewUpdateCommander
    {
        public abstract void Repaint();
        public abstract void Clear();
    }
}
