using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.Plugins.Common.DataDiff;

namespace Elektronik.Plugins.Common.Commands.TrackedObj
{
    public class RemoveTrackedObjCommands : RemoveCommand<SlamTrackedObject>
    {
        private readonly SortedDictionary<int, IList<SimpleLine>> _tracks = new ();

        private readonly ITrackedCloudContainer<SlamTrackedObject> _container;

        public RemoveTrackedObjCommands(ITrackedCloudContainer<SlamTrackedObject> container,
                                        SlamTrackedObject[] objects)
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
            _container.AddRangeWithHistory(Objs2Remove.OrderBy(i => i.Id).ToList(), _tracks.Values.ToList());
        }
    }

    public class RemoveTrackedObjDiffCommands : RemoveCommand<SlamTrackedObject, SlamTrackedObjectDiff>
    {
        private readonly SortedDictionary<int, IList<SimpleLine>> _tracks = new ();

        private readonly ITrackedCloudContainer<SlamTrackedObject> _container;

        public RemoveTrackedObjDiffCommands(ITrackedCloudContainer<SlamTrackedObject> container,
                                            SlamTrackedObjectDiff[] objects)
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
            _container.AddRangeWithHistory(Objs2Remove.OrderBy(i => i.Id).Select(d => _container[d.Id]).ToList(),
                                           _tracks.Values.ToList());
        }
    }
}