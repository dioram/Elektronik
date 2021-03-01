using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.Commands.Generic
{
    public class RemoveTrackedObjCommands : RemoveCommand<SlamTrackedObject>
    {
        private readonly SortedDictionary<int, IList<SlamLine>> _tracks = new SortedDictionary<int, IList<SlamLine>>();
        private readonly ITrackedContainer<SlamTrackedObject> _container;

        public RemoveTrackedObjCommands(ITrackedContainer<SlamTrackedObject> container,
                                        IEnumerable<SlamTrackedObject> objects)
                : base(container, objects)
        {
            _container = container;
        }

        public override void Execute()
        {
            foreach (var o in Objs2Remove)
            {
                _tracks[o.Id] = _container.GetHistory(o.Id);
            }

            base.Execute();
        }

        public override void UnExecute()
        {
            _container.AddRangeWithHistory(Objs2Remove.OrderBy(i => i.Id), _tracks.Values);
        }
    }
}