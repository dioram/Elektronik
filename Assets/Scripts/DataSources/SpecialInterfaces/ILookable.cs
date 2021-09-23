using UnityEngine;

namespace Elektronik.DataSources.SpecialInterfaces
{
    public interface ILookable
    {
        /// <summary> Returns coordinates of camera for best position to look at container's content. </summary>
        /// <param name="transform"> Initial camera transform. </param>
        /// <returns> End camera transform. </returns>
        (Vector3 pos, Quaternion rot) Look(Transform transform);
    }
}