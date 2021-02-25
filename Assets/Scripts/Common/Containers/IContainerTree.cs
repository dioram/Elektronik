using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public interface IContainerTree
    {
        string DisplayName { get; }
        
        IEnumerable<IContainerTree> Children { get; }

        bool IsActive { get; set; }

        void Clear();

        void SetRenderer(object renderer);
    }
}