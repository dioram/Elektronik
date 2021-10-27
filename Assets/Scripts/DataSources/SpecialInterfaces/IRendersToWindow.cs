using Elektronik.UI.Windows;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Marks that data source renders not to cloud but to window. </summary>
    public interface IRendersToWindow : IDataSource
    {
        /// <summary> Window where it will be rendered. </summary>
        /// <remarks> Needs to be set at <see cref="IDataSource.AddConsumer"/> </remarks>
        /// <example>
        /// public void AddConsumer(IDataConsumer consumer)
        /// {
        ///     if (consumer is WindowsManager factory)
        ///     {
        ///         factory.CreateWindow&lt;SlamInfoRenderer&gt;(DisplayName, (renderer, window) =>
        ///         {
        ///             _info = renderer;
        ///             Window = window;
        ///         });
        ///     }
        /// }
        /// </example>
        public Window Window { get; }
    }
}