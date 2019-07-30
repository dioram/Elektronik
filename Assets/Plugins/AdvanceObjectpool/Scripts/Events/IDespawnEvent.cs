using UnityEngine;

public interface IDespawnEvent
{
    void OnDespawned(GameObject targetGameObject, ObjectPool sender);
}
