using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    private static ObjectPool<T> _sharedInstance;
    public static ObjectPool<T> SharedInstance
    {
        get
        {
            if (_sharedInstance == null)
            {
                _sharedInstance = FindObjectOfType<ObjectPool<T>>();
                if (_sharedInstance == null)
                {
                    Debug.LogError("No instance of ObjectPool found.");
                }
            }
            return _sharedInstance;
        }
    }
    
    public T objectToPool;
    
    public int amount;
    
    public List<T> pooledObjects;
    
    
    private Transform PoolContainer => gameObject.transform;
    
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

    private int _count = 0;
    
    private void Awake()
    {
        if (_sharedInstance != null && _sharedInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _sharedInstance = this;
        }
    }

    private void Start()
    {
        pooledObjects = new();
        for (var i = 0; i < AmountToPool; i++)
        {
            AddObjectToPool();
        }
    }

    private T AddObjectToPool()
    {
        var tmp = Instantiate(
            objectToPool,
            PoolContainer.transform,
            true
            );
        tmp.name = $"{objectToPool.name}-{_count++}";
        tmp.gameObject.SetActive(false);
        pooledObjects.Add(tmp);
        return tmp;
    }

    public T GetPooledObject() {
        if (pooledObjects is null)
            return null;
        
        foreach (var t in pooledObjects.Where(t => !t.gameObject.activeInHierarchy))
        {
            return t;
        }

        // If no inactive object is found, add a new one to the pool and return it
        return AddObjectToPool();
    }

    public void ReturnToPool(T objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
    }

    public void OnSwapBuild(InputValue value) {
        var input = value.Get<float>();
        Debug.Log(input);
    }
}