using UnityEngine;

namespace Elektronik.DataSources.SpecialInterfaces
{
    /// <summary> Mark that this container can produce pose from where camera can see all of its content. </summary>
    public interface ILookableDataSource
    {
        /// <summary> Returns coordinates of camera for best position to look at container's content. </summary>
        /// <param name="transform"> Initial camera transform. </param>
        /// <returns> End camera transform. </returns>
        Pose Look(Transform transform);
    }
}