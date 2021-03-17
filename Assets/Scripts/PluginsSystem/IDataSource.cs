using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Presenters;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for plugins implements new data sources. </summary>
    public interface IDataSource : IElektronikPlugin
    {
        /// <summary> Converter for raw Vector3 and Quaternions </summary>
        [CanBeNull] ICSConverter Converter { get; set; }

        /// <summary> Containers with cloud data. </summary>
        [NotNull] IContainerTree Data { get; }
        
        /// <summary> Containers with any data. </summary>
        [CanBeNull] DataPresenter PresentersChain { get; }
    }
}