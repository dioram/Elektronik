using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Settings;

namespace Elektronik.PluginsSystem
{
    public interface IDataSource : IContainerTree, IElektronikPlugin
    {
        ICSConverter Converter { get; set; }
    }
}