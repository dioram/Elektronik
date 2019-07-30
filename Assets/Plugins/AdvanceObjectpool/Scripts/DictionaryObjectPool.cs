using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryObjectPool
{
    /// <summary>
    /// The list containing the object pools.
    /// </summary>
    private List<ObjectPool> _objectPools = new List<ObjectPool>();
    /// <summary>
    /// The list containing the lookupstring to find index of the objectpool.
    /// </summary>
    private List<string> _lookupString = new List<string>();

    /// <summary>
    /// Find a ObjectPool from a lookup name.
    /// </summary>
    /// <param name="value">The string to lookup in the objectpool list.</param>
    /// <returns>A objectpool object if any is related to the lookup string, otherwise null.</returns>
    public ObjectPool this[string value]
    {
        get
        {
            if (_lookupString.Contains(value))
            {
                return _objectPools[_lookupString.IndexOf(value)];
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Find a ObjectPool from an index.
    /// </summary>
    /// <param name="value">The index to get the objectpool from.</param>
    /// <returns>A objectpool object if any is related to the lookup string, otherwise null.</returns>
    /// <exception cref="System.IndexOutOfRangeException">If value is larger than the amount of objects in the dictionary.</exception>
    public ObjectPool this[int value]
    {
        get
        {
            return _objectPools[value];
        }
    }

    /// <summary>
    /// Adds a new objectpool to the dictionary.
    /// </summary>
    /// <param name="lookupName">The name used for looking up a pool.</param>
    /// <param name="_pool">An already created pool.</param>
    public void AddObjectPool(string lookupName, ObjectPool _pool)
    {
        _lookupString.Add(lookupName);
        _objectPools.Add(_pool);
    }
    /// <summary>
    /// Adds a new objectpool to the dictionary.
    /// </summary>
    /// <param name="lookupName">The name used for looking up a pool.</param>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    public void AddObjectPool(string lookupName, GameObject prefab)
    {
        AddObjectPool(lookupName, new ObjectPool(prefab));
    }

    /// <summary>
    /// Instantiate a new object pool with a prefab and a parent.
    /// </summary>
    /// <param name="lookupName">The name used for looking up a pool.</param>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    /// <param name="parent">The transform which should hold/be the parent of all the instantiated objects.</param>
    public void AddObjectPool(string lookupName, GameObject prefab, Transform parent)
    {
        AddObjectPool(lookupName, new ObjectPool(prefab, parent));
    }

    /// <summary>
    /// Adds a new objectpool to the dictionary.
    /// </summary>
    /// <param name="lookupName">The name used for looking up a pool.</param>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    /// <param name="preallocateAmount">The amount of object which should be prealocated.</param>
    public void AddObjectPool(string lookupName, GameObject prefab, int preallocateAmount)
    {
        AddObjectPool(lookupName, new ObjectPool(prefab, preallocateAmount));
    }

    /// <summary>
    /// Adds a new objectpool to the dictionary.
    /// </summary>
    /// <param name="lookupName">The name used for looking up a pool.</param>
    /// <param name="prefab">The target gameobject which should be cloned. Can not be null.</param>
    /// <param name="parent">The transform which should hold/be the parent of all the instantiated objects.</param>
    /// <param name="preallocateAmount">The amount of object which should be prealocated.</param>
    public void AddObjectPool(string lookupName, GameObject prefab, Transform parent, int preallocateAmount)
    {
        AddObjectPool(lookupName, new ObjectPool(prefab, parent, preallocateAmount));
    }

    /// <summary>
    /// Removes a pool from the dictionary.
    /// </summary>
    /// <param name="lookupName">The lookupname of the pool.</param>
    /// <returns>Returns true is the pool was removed, if not, returns false.</returns>
    public bool RemoveObjectPool(string lookupName)
    {
        return RemoveObjectPool(lookupName, false);
    }

    /// <summary>
    /// Removes a pool from the dictionary.
    /// </summary>
    /// <param name="lookupName">The lookupname of the pool.</param>
    /// <param name="destroyObject">Indicates if the objects of the pool should be destroy.</param>
    /// <returns>Returns true is the pool was removed, if not, returns false.</returns>
    public bool RemoveObjectPool(string lookupName, bool destroyObject)
    {
        if (_lookupString.Contains(lookupName))
        {
            return RemoveObjectPool(_lookupString.IndexOf(lookupName), false);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a pool from the dictionary.
    /// </summary>
    /// <param name="index">The index of the pool to remove.</param>
    /// <returns>Returns true is the pool was removed, if not, returns false.</returns>
    public bool RemoveObjectPool(int index)
    {
        return RemoveObjectPool(index, false);

    }

    /// <summary>
    /// Removes a pool from the dictionary.
    /// </summary>
    /// <param name="index">The index of the pool to remove.</param>
    /// <param name="destroyObject">Indicates if the objects of the pool should be destroy.</param>
    /// <returns>Returns true is the pool was removed, if not, returns false.</returns>
    public bool RemoveObjectPool(int index, bool destroyObject)
    {
        if (index >= _objectPools.Count)
        {
            if (destroyObject)
            {
                _objectPools[index].DespawnAllActiveObjects(true);
            }
            _lookupString.RemoveAt(index);
            _objectPools.RemoveAt(index);
            return true;
        }
        else
        {
            return false;
        }
    }
}
