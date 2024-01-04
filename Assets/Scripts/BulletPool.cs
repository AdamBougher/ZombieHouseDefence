using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public static BulletPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public Transform poolContainer;
    public int Amount;

    private int AmountToPool
    {
        get {
            if (Amount < 2)
            {
                Amount = 2;
            }
            return Amount;
        }
    }

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
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
        for (int i = 0; i < AmountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
