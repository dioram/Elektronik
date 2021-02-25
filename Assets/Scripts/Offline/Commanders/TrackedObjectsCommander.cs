using Elektronik.Common.Commands;
using Elektronik.Common.Commands.Generic;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Containers;

namespace Elektronik.Offline.Commanders
{
    public class TrackedObjectsCommander : Commander
    {
        public TrackedObjectsContainer Container;
        
        protected virtual ICommand GetCommand(IEnumerable<SlamTrackedObject> objs, PacketPb.Types.ActionType action)
        {
            switch (action)
            {
                case PacketPb.Types.ActionType.Add:
                    return new AddCommand<SlamTrackedObject>(Container, objs);
                case PacketPb.Types.ActionType.Update:
                    return new UpdateCommand<SlamTrackedObject>(Container, objs);
                case PacketPb.Types.ActionType.Remove:
                    return new RemoveTrackedObjCommands(Container, objs);
                case PacketPb.Types.ActionType.Clear:
                    return new ClearTrackedObjsCommand(Container);
                default: return null;
            }
        }

        public override void GetCommands(PacketPb pkg, in LinkedList<ICommand> commands)
        {
            if (pkg.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var command = GetCommand(pkg.ExtractTrackedObjects(Converter).ToList(), pkg.Action);
                command.Execute();
                commands.AddLast(command);
            }
            base.GetCommands(pkg, commands);
        }
    }
}
