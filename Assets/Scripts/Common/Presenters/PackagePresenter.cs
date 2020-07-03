using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Pb;
using UnityEngine;

namespace Elektronik.Common.Presenters
{
    public abstract class PackagePresenter : MonoBehaviour, IChainable<PackagePresenter>
    {
        protected PackagePresenter m_presenter;
        public IChainable<PackagePresenter> SetSuccessor(IChainable<PackagePresenter> presenter) => m_presenter = presenter as PackagePresenter;
        public abstract void Present(PacketPb package);
    }
}
