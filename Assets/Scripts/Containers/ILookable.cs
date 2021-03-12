using UnityEngine;

namespace Elektronik.Containers
{
    public interface ILookable
    {
        (Vector3 pos, Quaternion rot) Look(Transform transform);
    }
}