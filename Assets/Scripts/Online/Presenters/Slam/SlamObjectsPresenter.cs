using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Presenters;
using System.Linq;

namespace Elektronik.Online.Presenters.Slam
{
    public abstract class SlamObjectsPresenter<T> : RepaintablePackagePresenter
    {
        public override void Clear() => Map.Clear();
        public override abstract void Present(IPackage package);
        public override void Repaint() => Map.Repaint();
        public abstract IRepaintableContainer<T> Map { get; }
        protected abstract T Tint(T p);
        protected abstract T Move(T p);

        protected virtual void UpdateObjects(ActionDataPackage<T> package)
        {
            switch (package.ActionType)
            {
                case ActionType.Create:
                    Map.Add(package.Objects);
                    break;
                case ActionType.Tint:
                case ActionType.Fuse:
                    Map.Update(package.Objects.Select(Tint));
                    break;
                case ActionType.Move:
                    Map.Update(package.Objects.Select(Move));
                    break;
                case ActionType.Remove:
                    Map.Remove(package.Objects);
                    break;
                case ActionType.Clear:
                    Map.Clear();
                    break;
            }
        }
    }
}
