using Elektronik.Common.Data.Pb;
using UnityEngine;

namespace Elektronik.Common.Presenters
{
    public abstract class PackagePresenter : MonoBehaviour, IChainable<PackagePresenter>
    {
        protected PackagePresenter Successor;
        public IChainable<PackagePresenter> SetSuccessor(IChainable<PackagePresenter> presenter) => Successor = presenter as PackagePresenter;
        public abstract void Present(PacketPb package);
    }
}
