using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Presenters;

namespace Elektronik.PluginsSystem
{
    public interface IDataSource : IElektronikPlugin
    {
        ICSConverter Converter { get; set; }

        IContainerTree Data { get; }
        
        DataPresenter PresentersChain { get; }
    }
}