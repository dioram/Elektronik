using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Advance Object Pool/Scriptable Object Pool")]
public class ScriptableObjectObjectPool : ScriptableObject
{
    [SerializeField]
    ObjectPoolLookupName[] _serilizableObject;
    DictionaryObjectPool _objectPool;

    /// <summary>
    /// Gets the objects dictionary pool.
    /// </summary>
    public DictionaryObjectPool ObjectPool
    {
        get
        {
            return _objectPool;
        }
        private set
        {
            _objectPool = value;
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
            return ObjectPool[value];
        }
    }

    /// <summary>
    /// Find a ObjectPool from a lookup name.
    /// </summary>
    /// <param name="value">The string to lookup in the objectpool list.</param>
    /// <returns>A objectpool object if any is related to the lookup string, otherwise null.</returns>
    public ObjectPool this[string value]
    {
        get
        {
            return ObjectPool[value];
        }
    }

    /// <summary>
    /// Setup the object pool with <see cref="ObjectPool"/> added in the inspector. This call MUST be made for the pool to work, and should only be called once per pool's life cycle.
    /// </summary>
    public void Init()
    {
        Init(null, true);
    }

    /// <summary>
    /// Setup the object pool with <see cref="ObjectPool"/> added in the inspector. This call MUST be made for the pool to work, and should only be called once per pool's life cycle.
    /// </summary>
    /// <param name="mainParent">The target transform which should be parent to spawned items.</param>
    public void Init(Transform mainParent)
    {
        Init(mainParent, true);
    }
    /// <summary>
    /// Setup the object pool with <see cref="ObjectPool"/> added in the inspector. This call MUST be made for the pool to work, and should only be called once per pool's life cycle.
    /// </summary>
    /// <param name="sortInSubParents">Indicate if there should be created a subgroup for each object that is spawned</param>
    public void Init(bool sortInSubParents)
    {
        Init(null, sortInSubParents);
    }
    /// <summary>
    /// Setup the object pool with <see cref="ObjectPool"/> added in the inspector. This call MUST be made for the pool to work, and should only be called once per pool's life cycle.
    /// </summary>
    /// <param name="mainParent">The target transform which should be parent to spawned items.</param>
    /// <param name="sortInSubParents">Indicate if there should be created a subgroup for each object that is spawned</param>
    public void Init(Transform mainParent, bool sortInSubParents = true)
    {
        if (_objectPool == null)
        {
            _objectPool = new DictionaryObjectPool();
            Transform realParent = null;
            foreach (ObjectPoolLookupName opln in _serilizableObject)
            {
                if (opln.SerilizableObject.Prefab == null)
                {
                    Debug.LogError("Prefeb in Object target object for scriptableobject object pool " + name + " was null. Check your setup of the scriptable object.");
                    Debug.LogError("Object pool " + name + " was not initialized correctly!");
                    return;
                }
                if (sortInSubParents)
                {
                    realParent = new GameObject().transform;
                    realParent.name = (string.IsNullOrEmpty(opln.Name) ? opln.SerilizableObject.name : opln.Name);
                    realParent.parent = mainParent;
                }
                else
                {
                    realParent = mainParent;
                }
                _objectPool.AddObjectPool((string.IsNullOrEmpty(opln.Name) ? opln.SerilizableObject.name : opln.Name), opln.SerilizableObject.Prefab, realParent, opln.SerilizableObject.PreallocateAmount);
            }
        }
    }
}

[System.Serializable]
public class ObjectPoolLookupName
{
    [SerializeField]
    [Tooltip("The name accessor used for the pool, if null or empty the name of the ObjectPoolTarget Scriptableobject asset is used.")]
    string _name;
    [SerializeField]
    [Tooltip("The object which should be spawned.")]
    ObjectPoolTarget _serilizableObject;

    public string Name
    {
        get
        {
            return _name;
        }

        private set
        {
            _name = value;
        }
    }

    public ObjectPoolTarget SerilizableObject
    {
        get
        {
            return _serilizableObject;
        }

        private set
        {
            _serilizableObject = value;
        }
    }
}