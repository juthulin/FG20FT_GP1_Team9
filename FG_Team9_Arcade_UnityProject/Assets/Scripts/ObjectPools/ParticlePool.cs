using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class ParticlePool : ObjectPool<ParticleType>
{
    [SerializeField] List<ParticlePoolItem> particlesToPool;

    public static ParticlePool Instance { get; private set; }
    
    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    protected override void Start()
    {
        listOfPools = new List<Pool>();

        foreach (ParticlePoolItem item in particlesToPool)
        {
            Pool pool = new Pool();
            listOfPools.Add(pool);
            mappedPools.Add(item.objectType, pool);

            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = Instantiate(item.objectToPool, transform);
                obj.SetActive(false);
                pool.pooledObjects.Add(obj);
            }
        }
    }

    public override GameObject GetPooledObject(in ParticleType objectType)
    {
        if (!mappedPools.ContainsKey(objectType)) return null;

        List<GameObject> specifiedPool = mappedPools[objectType].pooledObjects;

        for (int i = 0; i < specifiedPool.Count; i++)
        {
            if (!specifiedPool[i].activeInHierarchy)
            {
                return specifiedPool[i];
            }
        }

        foreach (ParticlePoolItem item in particlesToPool)
        {
            if (item.objectType == objectType && item.poolCanExpand)
            {
                GameObject obj = Instantiate(item.objectToPool, transform);
                obj.SetActive(false);
                specifiedPool.Add(obj);
                return obj;
            }
        }

        return null;
    }
}
