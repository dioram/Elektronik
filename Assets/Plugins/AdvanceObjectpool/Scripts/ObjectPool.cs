using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class ObjectPool
{
    /// <summary>
    /// Event(s) which is invoked when a object is spawned.
    /// </summary>
    public event SpawnEvent OnObjectSpawn;
    /// <summary>
    /// Event(s) which is invoked when a object is despawned.
    /// </summary>
    public event DespawnEvent OnObjectDeSpawn;

    /// <summary>
    /// A list over all the active object in this pool. - NOTE: You should NEVER destroy any object(s) instantiate/spawned with this object pool as this may cause errors.
    /// </summary>
    private List<GameObject> ActiveObject { get; set; }
    /// <summary>
    /// A queue over all despawned objects.
    /// </summary>
    private Queue<GameObject> DespawnedElements { get; set; }
    /// <summary>
    /// The object this pool should handled.
    /// </summary>
    private GameObject Prefab { get; set; }
    /// <summary>
    /// The parent that hold the game objects, if null no parent is assigned.
    /// </summary>
    private Transform Parent { get; set; }

    /// <summary>
    /// Gets the numbers of active objects.
    /// </summary>
    public int Count
    {
        get
        {
            return ActiveObject.Count;
        }
    }

    /// <summary>
    /// Gets the numbers of inactive objects.
    /// </summary>
    public int CountInactive
    {
        get
        {
            return DespawnedElements.Count;
        }
    }

    /// <summary>
    /// Gets the combined numbers of active and inactive objects.
    /// </summary>
    public int CountAll
    {
        get
        {
            return ActiveObject.Count + DespawnedElements.Count;
        }
    }

    /// <summary>
    /// Instantiate a new object pool with a prefab
    /// </summary>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    public ObjectPool(GameObject prefab)
    {
        if (prefab == null)
            throw new System.NullReferenceException("The parameter 'prefab' can not be null in the objectpool.");

        Prefab = prefab;
        ActiveObject = new List<GameObject>();
        DespawnedElements = new Queue<GameObject>();
    }

    /// <summary>
    /// Instantiate a new object pool with a prefab and a parent.
    /// </summary>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    /// <param name="parent">The transform which should hold/be the parent of all the instantiated objects.</param>
    public ObjectPool(GameObject prefab, Transform parent) : this (prefab)
    {
        Parent = parent;
    }

    /// <summary>
    /// Instantiate a new object pool with a prefab and prealocates a desired number(s) of that object.
    /// </summary>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    /// <param name="preallocateAmount">The amount of object which should be prealocated.</param>
    public ObjectPool(GameObject prefab, int preallocateAmount) : this (prefab)
    {
        for (int i = 0; i < preallocateAmount; i++)
        {
            DespawnedElements.Enqueue(Instantiate());
        }
    }

    /// <summary>
    /// Instantiate a new object pool with a prefab and a parent and prealocates a desired number(s) of that object.
    /// </summary>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    /// <param name="parent">The transform which should hold/be the parent of all the instantiated objects.</param>
    /// <param name="preallocateAmount">The amount of object which should be prealocated.</param>
    public ObjectPool(GameObject prefab, Transform parent, int preallocateAmount) : this (prefab)
    {
        Parent = parent;
        for (int i = 0; i < preallocateAmount; i++)
        {
            DespawnedElements.Enqueue(Instantiate());
        }
    }

    /// <summary>
    /// Checks if a gameobject is in the objectpool.
    /// </summary>
    /// <param name="item">The gameobject to check.</param>
    /// <returns>True if the gameobject is in the objectpool, otherwise false.</returns>
    public bool Contains(GameObject item)
    {
        return (ActiveObject.Contains(item) || DespawnedElements.Contains(item));
    }

    /// <summary>
    /// Checks if a gameobject is in the active objectpool.
    /// </summary>
    /// <param name="item">The gameobject to check.</param>
    /// <returns>True if the gameobject is active in the objectpool, otherwise false.</returns>
    public bool ContainsActive(GameObject item)
    {
        return ActiveObject.Contains(item);
    }

    /// <summary>
    /// Checks if a gameobject is in the inactive objectpool.
    /// </summary>
    /// <param name="item">The gameobject to check.</param>
    /// <returns>True if the gameobject is in the inactive objectpool, otherwise false.</returns>
    public bool ContainsInactive(GameObject item)
    {
        return ActiveObject.Contains(item);
    }

    /// <summary>
    /// Spawns a gameobject.
    /// </summary>
    /// <returns>The spawned gameobject.</returns>
    public GameObject Spawn()
    {
        return Spawn(Vector3.zero);
    }

    /// <summary>
    /// Spawns a gameobject.
    /// </summary>
    /// <param name="pos">The position to spawn the object.</param>
    /// <returns>The spawned gameobject.</returns>
    public GameObject Spawn(Vector3 pos)
    {
        return Spawn(pos, Quaternion.identity);
    }

    /// <summary>
    /// Spawns a gameobject.
    /// </summary>
    /// <param name="pos">The position to spawn the gameobject.</param>
    /// <param name="rot">The rotation to spawn the gameobject.</param>
    /// <returns>The spawned gameobject.</returns>
    public GameObject Spawn(Vector3 pos, Quaternion rot)
    {
        GameObject com = null;
        if (DespawnedElements.Count != 0)
        {
            while (com == null && DespawnedElements.Count > 0)
            {
                com = DespawnedElements.Dequeue();
            }
            if (com == null)
            {
                com = Instantiate();
            }
        }
        else
        {
            com = Instantiate();
        }
        com.transform.position = pos;
        com.transform.transform.rotation = rot;
        ActiveObject.Add(com);
        com.gameObject.SetActive(true);
        if (OnObjectSpawn != null)
        {
            OnObjectSpawn.Invoke(com, this);
        }
        return com;
    }

    /// <summary>
    /// Despawns a object.
    /// </summary>
    /// <param name="pObject">The object to despawn.</param>
    /// <returns>True if the object was removed correctly.</returns>
    public bool Despawn(GameObject pObject)
    {
        return Despawn(pObject, false);
    }

    /// <summary>
    /// Despawns a object.
    /// </summary>
    /// <param name="pObject">The object to despawn.</param>
    /// <param name="destroyObject">Should the despawned object also get destroyed?</param>
    /// <returns>True if the object was removed correctly.</returns>
    public bool Despawn(GameObject pObject, bool destroyObject)
    {
        return DespawnInternal(pObject, destroyObject, true);
    }


    /// <summary>
    /// Despawns a object.
    /// </summary>
    /// <param name="pObject">The object to despawn.</param>
    /// <param name="destroyObject">Should the despawned object also get destroyed?</param>
    /// <param name="nullcheck">Should the list get interated to removed null values?</param>
    /// <returns>True if the object was removed correctly.</returns>
    private bool DespawnInternal(GameObject pObject, bool destroyObject, bool nullcheck)
    {
        if (nullcheck)
        {
            ActiveObject.RemoveAll(g => g == null);
        }
        if (ActiveObject.Remove(pObject))
        {
            if (OnObjectDeSpawn != null)
            {
                OnObjectDeSpawn.Invoke(pObject, this);
            }
            if (destroyObject)
            {
                if (pObject.GetComponent<IDespawnEvent>() != null)
                {
                    OnObjectDeSpawn -= new DespawnEvent(pObject.GetComponent<IDespawnEvent>().OnDespawned);
                }
                Destroy(pObject);
            }
            else
            {
                pObject.SetActive(false);
                DespawnedElements.Enqueue(pObject);
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Despawns all the active objects and tell rather they should be enqueued for later use or completely destroyed.
    /// </summary>
    /// <returns>True if all objects were despawed correctly.</returns>
    public bool DespawnAllActiveObjects()
    {
        return DespawnAllActiveObjects(false);
    }

    /// <summary>
    /// Despawns all the active objects and tell rather they should be enqueued for later use or completely destroyed.
    /// </summary>
    /// <param name="destroyObject">Should the despawned objects also get destroyed?</param>
    /// <returns>True if all objects were despawed correctly.</returns>
    public bool DespawnAllActiveObjects(bool destroyObject)
    {
        ActiveObject.RemoveAll(g => g == null);

        while (ActiveObject.Count != 0)
        {
            if (!DespawnInternal(ActiveObject[0], destroyObject, false))
            {
                return false;
            }

        }
        return true;
    }

    /// <summary>
    /// Instantiate a new Gameobject is the poolqueue is empty.
    /// </summary>
    /// <returns>The new gameobject.</returns>
    private GameObject Instantiate()
    {
        GameObject go = (GameObject)GameObject.Instantiate(Prefab);

        if (Parent != null)
        {
            go.transform.parent = Parent;
        }
        if (go.GetComponent<ISpawnEvent>() != null)
        {
            OnObjectSpawn += new SpawnEvent(go.GetComponent<ISpawnEvent>().OnSpawned);
        }

        go.gameObject.SetActive(false);


        return go;
    }

    /// <summary>
    /// Destroys a object from the pool.
    /// </summary>
    /// <param name="pObject">The gameobject to destroy.</param>
    private void Destroy(GameObject pObject)
    {
        GameObject.Destroy(pObject);
    }

    /// <summary>
    /// Allocates an amount of gameobjects to the pool queue.
    /// </summary>
    /// <param name="amount">The numbers of objects to allocate.</param>
    public void Allocate(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject com = (GameObject)GameObject.Instantiate(Prefab);
            com.gameObject.SetActive(false);
            if (Parent != null)
            {
                com.transform.parent = Parent;
            }
            DespawnedElements.Enqueue(com);
        }
    }

    /// <summary>
    /// Deallocates an amount of gameobjects from the pool queue.
    /// </summary>
    /// <param name="amount">The numbers of objects to deallocate, if larger then present gameobject, all gameobject is removed still.</param>
    public void DeAllocate(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (DespawnedElements.Count == 0)
                break;

            Destroy(DespawnedElements.Dequeue());
        }
    }
}

/// <summary>
/// Delegate that is called when a objectpool spawns a gameobject.
/// </summary>
/// <param name="targetGameObject">The gameobject which got spawned.</param>
/// <param name="sender">The objectpool which spawned the gameobject.</param>
public delegate void SpawnEvent(GameObject targetGameObject, ObjectPool sender);
/// <summary>
/// Delegate that is called when a objectpool despawns a gameobject.
/// </summary>
/// <param name="targetGameObject">The gameobject which got despawned.</param>
/// <param name="sender">The objectpool which despawned the gameobject.</param>
public delegate void DespawnEvent(GameObject targetGameObject, ObjectPool sender);