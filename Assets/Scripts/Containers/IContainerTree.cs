using System.Collections.Generic;
using JetBrains.Annotations;

namespace Elektronik.Containers
{
    public interface IContainerTree
    {
        [NotNull] string DisplayName { get; set; }

        [NotNull] IEnumerable<IContainerTree> Children { get; }

        bool IsActive { get; set; }

        void Clear();

        void SetRenderer(object renderer);
    }
}