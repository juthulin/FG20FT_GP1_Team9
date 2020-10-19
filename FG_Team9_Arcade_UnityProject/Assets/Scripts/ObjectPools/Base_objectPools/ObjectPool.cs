using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour
{
    protected Dictionary<T, Pool> mappedPools = new Dictionary<T, Pool>();
    
    [SerializeField] protected List<Pool> listOfPools;

    protected abstract void Awake();

    protected abstract void Start();

    public abstract GameObject GetPooledObject(in T objectType);

}
