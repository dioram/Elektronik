using Elektronik.Common.Data;
using UnityEngine;

namespace Elektronik.Common
{
    public abstract class PackagePresenter : MonoBehaviour, IChainable<PackagePresenter>
    {
        protected PackagePresenter m_presenter;
        public IChainable<PackagePresenter> SetSuccessor(IChainable<PackagePresenter> presenter) => m_presenter = presenter as PackagePresenter;
        public abstract void Present(IPackage package);
    }
}
