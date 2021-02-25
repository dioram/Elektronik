using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Common.Commands.Generic
{
    public class ClearTrackedObjsCommand : ClearCommand<SlamTrackedObject>
    {
        private readonly SortedDictionary<int, (SlamTrackedObject, IList<SlamLine>)> _tracks
                = new SortedDictionary<int, (SlamTrackedObject, IList<SlamLine>)>();

        private readonly ITrackedContainer<SlamTrackedObject> _container;

        public ClearTrackedObjsCommand(ITrackedContainer<SlamTrackedObject> container)
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
            _container.AddRangeWithHistory(_tracks.Values.Select(p => p.Item1),
                                           _tracks.Values.Select(p => p.Item2));
        }
    }
}