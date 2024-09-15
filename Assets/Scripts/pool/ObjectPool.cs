using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ObjectPool<T> : MonoBehaviour
{
    public static ObjectPool<T> SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public Transform poolContainer;
    [FormerlySerializedAs("Amount")] public int amount;

    protected int AmountToPool
    {
        get {
            if (amount < 2)
            {
                amount = 2;
            }
            return amount;
        }
        set => amount = value;
    }

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
            tmp.transform.parent = poolContainer.transform;
        }
    }

    public GameObject GetPooledObject()
    {
        for (var i = 0; i < AmountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
    
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
