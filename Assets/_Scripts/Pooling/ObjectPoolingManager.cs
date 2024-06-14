using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    [SerializeField] private ObjectPool[] pools = null;

    public static ObjectPool[] CurrentPools = null;
    private void Start()
    {
        InitObjectPools(pools, transform);
    }
    private static void InitObjectPools(ObjectPool[] pools, Transform instance)
    {
        if(CurrentPools != null)
        {
            foreach(ObjectPool pool in CurrentPools)
            {
                pool.Clear();
            }
        }
        CurrentPools = pools;
        foreach (ObjectPool pool in pools)
        {
            Transform pooledParent = new GameObject().transform;
            pooledParent.name = "Pool_" + pool.Prefab.name;
            pooledParent.SetParent(instance, false);
            pool.Init(pooledParent);
        }
    }
    public static GameObject GetObject(GameObject prefab)
    {
        foreach(ObjectPool pool in CurrentPools)
        {
            if (pool.Prefab != prefab) continue;
            return pool.GetInstance();
        }
        Debug.Log("No such prefab in pools " + prefab);
        return null;
    }
    public static GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetObject(prefab);
        if (obj == null) return null;

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }
    public static void ReturnObject(GameObject instance)
    {
        foreach(ObjectPool pool in CurrentPools)
        {
            if (pool.FromThisPool(instance)) 
            {
                pool.ReturnInstance(instance);
                return;
            }
        }
        Debug.Log("Cannot find the pool for " + instance.name);
    }
    private void OnDestroy()
    {
        CurrentPools = null;
    }
    [Serializable]
    public class ObjectPool
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;

        private Transform parent = null;
        private List<GameObject> availableObjects = new List<GameObject>();
        private List<GameObject> allObjects = new List<GameObject>();
        public GameObject Prefab => prefab;

        public void Init(Transform parent)
        {
            this.parent = parent;
            for(int i = 0; i < initialSize; i++)
            {
                CreateNewInstance(parent);
            }
        }
        public GameObject GetInstance()
        {
            if(availableObjects.Count > 0)
            {
                GameObject obj = availableObjects[0];
                obj.transform.SetParent(null, true);
                obj.SetActive(true);
                availableObjects.RemoveAt(0);
                return obj;
            }
            else
            {
                return CreateNewInstance(parent);
            }
        }
        public void ReturnInstance(GameObject obj)
        {
            if (availableObjects.Contains(obj))
            {
                Debug.Log("Added duplicate to pool " + obj.name);
                return;
            }

            obj.SetActive(false);
            availableObjects.Add(obj);
            obj.transform.SetParent(parent, true);
        }
        private GameObject CreateNewInstance(Transform parent)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.name += "_Pooled (" + allObjects.Count + ")";
            obj.SetActive(false);
            availableObjects.Add(obj);
            allObjects.Add(obj);
            return obj;
        }
        public bool FromThisPool(GameObject obj)
        {
            return allObjects.Contains(obj);
        }
        public void Clear()
        {
            foreach(GameObject obj in availableObjects)
            {
                Destroy(obj);
            }
            availableObjects.Clear();
            Destroy(parent.gameObject);
        }
    }
}
