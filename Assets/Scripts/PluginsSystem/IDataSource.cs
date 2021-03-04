using Elektronik.Containers;
using Elektronik.Data.Converters;

namespace Elektronik.PluginsSystem
{
    public interface IDataSource : IElektronikPlugin
    {
        ICSConverter Converter { get; set; }

        IContainerTree Data { get; }
    }
}