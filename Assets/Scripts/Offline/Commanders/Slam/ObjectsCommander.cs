using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Linq;

namespace Elektronik.Offline.Commanders.Slam
{
    public abstract class ObjectsCommander<T> :  PackageViewUpdateCommander
    {
        public abstract IRepaintableContainer<T> ObjectsMap { get; }
        public abstract T Move(T obj);
        public abstract T Tint(T obj);
        public abstract T PostProcessTint(T obj);
        public virtual T ClearTint(T obj) => Tint(obj);
        public virtual IPackageViewUpdateCommand GetCommand(ISlamActionPackage pkg)
        {
            var concretePkg = pkg as ActionDataPackage<T>;
            if (concretePkg == null)
                return null;
            switch (concretePkg.ActionType)
            {
                case ActionType.Create:
                    return new AddCommand<T>(ObjectsMap, concretePkg.Objects);
                case ActionType.Tint:
                case ActionType.Fuse:
                case ActionType.Remove: // We want to show, which objects will be removed. The objects will be really removed while post processing.
                    return new UpdateCommand<T>(ObjectsMap, concretePkg.Objects.Select(Tint));
                case ActionType.Move:
                    return new UpdateCommand<T>(ObjectsMap, concretePkg.Objects.Select(Move));
                case ActionType.Clear:
                    return new UpdateCommand<T>(ObjectsMap, ObjectsMap.Select(ClearTint));
                default:
                    return null;
            }
        }
        public virtual IPackageViewUpdateCommand GetPostProcessCommand(ISlamActionPackage pkg)
        {
            var concretePkg = pkg as ActionDataPackage<T>;
            if (concretePkg == null)
                return null;
            switch (concretePkg.ActionType)
            {
                case ActionType.Tint:
                case ActionType.Fuse:
                    return new UpdateCommand<T>(ObjectsMap, concretePkg.Objects.Select(PostProcessTint));
                case ActionType.Remove:
                    return new RemoveCommand<T>(ObjectsMap, concretePkg.Objects);
                case ActionType.Clear:
                    return new ClearCommand<T>(ObjectsMap);
                default:
                    return null;
            }
        }

    }
}
