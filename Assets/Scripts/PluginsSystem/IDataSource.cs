using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;

namespace Elektronik.PluginsSystem
{
    public interface IDataSource : IElektronikPlugin
    {
        ICSConverter Converter { get; set; }

        IContainerTree Data { get; }
    }
}