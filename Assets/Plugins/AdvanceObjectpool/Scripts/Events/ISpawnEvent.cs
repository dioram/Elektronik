using UnityEngine;

public interface ISpawnEvent
{
    void OnSpawned(GameObject targetGameObject, ObjectPool sender);
}
