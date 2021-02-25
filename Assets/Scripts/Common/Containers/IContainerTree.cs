using Elektronik.Common.Clouds;

namespace Elektronik.Common.Containers
{
    public interface IContainerTree
    {
        string DisplayName { get; }
        
        IContainerTree[] Children { get; }

        void SetActive(bool active);

        void Clear();
    }
}