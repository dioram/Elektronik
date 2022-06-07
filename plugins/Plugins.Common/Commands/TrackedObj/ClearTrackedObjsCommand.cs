using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.Commands.Generic;

namespace Elektronik.Plugins.Common.Commands.TrackedObj
{
    public class ClearTrackedObjsCommand : ClearCommand<SlamTrackedObject>
    {
        private readonly SortedDictionary<int, (SlamTrackedObject, IList<SimpleLine>)> _tracks = new ();

        private readonly ITrackedCloudContainer<SlamTrackedObject> _container;

        public ClearTrackedObjsCommand(ITrackedCloudContainer<SlamTrackedObject> container)
                : base(container)
        {
            _container = container;
        }

        public override void Execute()
        {
            foreach (var o in _container)
            {
                _tracks[o.Id] = (o, _container.GetHistory(o.Id));
            }

            base.Execute();
        }

        public override void UnExecute()
        {
            _container.AddRangeWithHistory(_tracks.Values.Select(p => p.Item1).ToList(),
                                           _tracks.Values.Select(p => p.Item2).ToList());
        }
    }
}