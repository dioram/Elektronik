using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Advance Object Pool/Object Pool Targets")]
public class ObjectPoolTarget : ScriptableObject
{
    [SerializeField]
    [Tooltip("The prefab which should be cloned/spawned in the pool")]
    GameObject prefab;
    [SerializeField]
    [Tooltip("The amount of objects to be spawned from the start")]
    int preallocateAmount;

    public GameObject Prefab
    {
        get
        {
            return prefab;
        }
        private set
        {
            prefab = value;
        }
    }

    public int PreallocateAmount
    {
        get
        {
            return preallocateAmount;
        }
        private set
        {
            preallocateAmount = value;
        }
    }
}
