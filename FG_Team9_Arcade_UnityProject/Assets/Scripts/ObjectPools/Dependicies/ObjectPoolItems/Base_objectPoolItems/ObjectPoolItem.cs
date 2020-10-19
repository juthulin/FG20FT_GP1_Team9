using System;
using UnityEngine;

[Serializable]
public abstract class ObjectPoolItem<T>
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool poolCanExpand;
    public T objectType;
}
