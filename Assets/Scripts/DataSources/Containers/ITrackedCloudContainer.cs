using System.Collections.Generic;
using Elektronik.DataObjects;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Interface for container which items should have history of their positions. </summary>
    /// <typeparam name="TCloudItem"> Type of cloud items. </typeparam>
    public interface ITrackedCloudContainer<TCloudItem> : ICloudContainer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        /// <summary>
        /// Returns history of item with given id as list of lines connecting its previous positions.
        /// </summary>
        /// <param name="id"> Id of item from this container. </param>
        /// <returns> List of lines connecting item's previous positions </returns>
        IList<SimpleLine> GetHistory(int id);

        /// <summary> Adds new item with it's previous positions. </summary>
        /// <param name="item"> Added item. </param>
        /// <param name="history"> History of added item. </param>
        void AddWithHistory(TCloudItem item, IList<SimpleLine> history);

        /// <summary> Adds new items with theirs previous positions. </summary>
        /// <param name="items"> Added items. </param>
        /// <param name="histories"> History of added items. </param>
        void AddRangeWithHistory(IList<TCloudItem> items, IList<IList<SimpleLine>> histories);
    }
}