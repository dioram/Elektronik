using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.Plugins.Common.Commands.TrackedObj;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Plugins.Common.Parsing;
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
        
        public override ICommand? GetCommand(PacketPb pkg)
        {
            if (pkg.DataCase != PacketPb.DataOneofCase.TrackedObjs) return base.GetCommand(pkg);

            var objs = pkg.ExtractTrackedObjects(Converter);
            if (objs.Length == 0) return null;
            switch (pkg.Action)
            {
            case PacketPb.Types.ActionType.Add:
                return new AddCommand<SlamTrackedObject, SlamTrackedObjectDiff>(_container, objs);
            case PacketPb.Types.ActionType.Update:
                return new UpdateCommand<SlamTrackedObject, SlamTrackedObjectDiff>(_container, objs);
            case PacketPb.Types.ActionType.Remove:
                return new RemoveTrackedObjDiffCommands(_container, objs);
            case PacketPb.Types.ActionType.Clear:
                return new ClearTrackedObjsCommand(_container);
            default: 
                return null;
            }

        }
    }
}