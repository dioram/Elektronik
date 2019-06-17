using Elektronik.Common.Containers;
using Elektronik.Common.Data;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Tracking
{
    public class UpdateCommand : IPackageViewUpdateCommand
    {
        private Helmet m_helmet;
        private TrackingPackage m_pkg;

        public UpdateCommand(Helmet helmet, TrackingPackage package)
        {
            m_helmet = helmet;
            m_pkg = package;
        }

        public void Execute()
        {
            m_helmet.ReplaceAbs(m_pkg.pos, m_pkg.rot);
        }
        public void UnExecute()
        {
            m_helmet.TurnBack();
        }
    }
}
