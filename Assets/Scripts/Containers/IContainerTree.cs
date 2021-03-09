using System.Collections.Generic;

namespace Elektronik.Containers
{
    public interface IContainerTree
    {
        string DisplayName { get; set; }
        
        IEnumerable<IContainerTree> Children { get; }

        bool IsActive { get; set; }

        void Clear();

        void SetRenderer(object renderer);
    }
}