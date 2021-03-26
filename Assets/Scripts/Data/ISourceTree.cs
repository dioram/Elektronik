using System.Collections.Generic;
using JetBrains.Annotations;

namespace Elektronik.Containers
{
    /// <summary> Data sources (eg. Containers) with this interface can be united in tree structure. </summary>
    public interface ISourceTree
    {
        /// <summary> Display name of data source. </summary>
        /// <remarks> Will be used in SourceTree UI widget. </remarks>
        [NotNull]
        string DisplayName { get; set; }

        [NotNull] IEnumerable<ISourceTree> Children { get; }

        /// <summary> Should content of this data source be displayed or not. </summary>
        bool IsActive { get; set; }

        /// <summary> Clear all content. </summary>
        void Clear();

        /// <summary> Sets renderer class for this data source. </summary>
        /// <remarks> You should choose and set only correct type of renderer in implementation of this method. </remarks>
        /// <example>
        /// if (renderer is ICloudRenderer&lt;TCloudItem&gt; typedRenderer)
        /// {
        ///     OnAdded += typedRenderer.OnItemsAdded;
        ///     OnUpdated += typedRenderer.OnItemsUpdated;
        ///     OnRemoved += typedRenderer.OnItemsRemoved;
        ///     if (Count > 0)
        ///     {
        ///         OnAdded?.Invoke(this, new AddedEventArgs&lt;TCloudItem&gt;(this));
        ///     }
        /// }
        /// </example>
        /// <param name="renderer"> Content renderer </param>
        void SetRenderer(object renderer);
    }
}