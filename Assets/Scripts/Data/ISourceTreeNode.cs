using System.Collections.Generic;
using Elektronik.Renderers;
using JetBrains.Annotations;

namespace Elektronik.Data
{
    /// <summary> Data sources (eg. Containers) with this interface can be united in tree structure. </summary>
    public interface ISourceTreeNode
    {
        /// <summary> Display name of data source. </summary>
        /// <remarks> Will be used in SourceTree UI widget. </remarks>
        [NotNull]
        string DisplayName { get; set; }

        /// <summary> Child nodes of this source tree element. </summary>
        [NotNull] IEnumerable<ISourceTreeNode> Children { get; }

        /// <summary> Clear all content. </summary>
        void Clear();

        /// <summary> Add renderer for this data source. </summary>
        /// <remarks>
        /// You should choose and set only correct type of renderer in implementation of this method.
        /// Also you should propagate this call to children. 
        /// </remarks>
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
        /// foreach (var child in Children)
        /// {
        ///     child.AddRenderer(renderer);
        /// }
        /// </example>
        /// <param name="renderer"> Content renderer. </param>
        void AddRenderer(ISourceRenderer renderer);

        /// <summary> Removes renderer from this data source. </summary>
        /// <remarks> In implementation of this method you should propagate this call to children. </remarks>
        /// <param name="renderer"> Content renderer. </param>
        void RemoveRenderer(ISourceRenderer renderer);
    }
}