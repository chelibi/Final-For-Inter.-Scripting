using System.Collections.Generic;
using UnityEngine;

public class TurretPool : MonoBehaviour
{
    public static TurretPool Instance;

    [SerializeField] private GameObject pooledObjectPrefab;
    [SerializeField] private int pooledAmount = 20;
    private List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this;
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObjectPrefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        GameObject obj = Instantiate(pooledObjectPrefab);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }
}
