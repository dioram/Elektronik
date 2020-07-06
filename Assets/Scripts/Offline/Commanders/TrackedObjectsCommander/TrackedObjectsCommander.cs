using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;

namespace Elektronik.Offline.Commanders.TrackedObjectsCommander
{
    public partial class TrackedObjectsCommander : PackageViewUpdateCommander
    {
        public SlamMap map;
        
        protected virtual IPackageViewUpdateCommand GetCommand(IEnumerable<TrackedObjPb> objects, PacketPb.Types.ActionType action)
        {
            switch (action)
            {
                case PacketPb.Types.ActionType.Add:
                    return new AddCommand<TrackedObjPb>(map.TrackedObjsContainer, objects);
                case PacketPb.Types.ActionType.Update:
                    return new TrackedObjUpdate(map.TrackedObjsContainer, objects);
                case PacketPb.Types.ActionType.Remove:
                    return new TrackedObjRemove(map.TrackedObjsContainer, objects);
                case PacketPb.Types.ActionType.Clear:
                    return new TrackedObjClear(map.TrackedObjsContainer);
                default: return null;
            }
        }

        public override void GetCommands(PacketPb pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.DataCase == PacketPb.DataOneofCase.TrackedObjsPacket)
            {
                var command = GetCommand(pkg.TrackedObjsPacket.TrackedObjs, pkg.Action);
                command.Execute();
                commands.AddLast(command);
            }
            base.GetCommands(pkg, commands);
        }
    }
}
