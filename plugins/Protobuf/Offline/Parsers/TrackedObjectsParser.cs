using System.Collections.Generic;
using System.Linq;
using Elektronik.Commands;
using Elektronik.Commands.Generic;
using Elektronik.Commands.TrackedObj;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Offline;
using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Offline.Parsers
{
    public class TrackedObjectsParser : DataParser<PacketPb>
    {
        private readonly ITrackedContainer<SlamTrackedObject> _container;

        public TrackedObjectsParser(ITrackedContainer<SlamTrackedObject> container)
        {
            _container = container;
        }

        protected virtual ICommand? GetCommand(IList<SlamTrackedObjectDiff>? objs, PacketPb.Types.ActionType action)
        {
            if (objs is null || objs.Count == 0) return null;
            switch (action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<SlamTrackedObject, SlamTrackedObjectDiff>(_container, objs);
            case PacketPb.Types.ActionType.Update:
                return new UpdateCommand<SlamTrackedObject, SlamTrackedObjectDiff>(_container, objs);
            case PacketPb.Types.ActionType.Remove:
                return new RemoveTrackedObjDiffCommands(_container, objs);
            case PacketPb.Types.ActionType.Clear:
                return new ClearTrackedObjsCommand(_container);
            default: return null;
            }
        }

        public override ICommand? GetCommand(PacketPb pkg)
        {
            if (pkg.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var command = GetCommand(pkg.ExtractTrackedObjects(Converter), pkg.Action);
                return command;
            }

            return base.GetCommand(pkg);
        }
    }
}