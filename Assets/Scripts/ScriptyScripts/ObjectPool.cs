using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectToPool; // The prefab that will be pooled
    public int amountToPool; // Number of objects to pool
    public float spawnRate = 2f; // Time interval between spawns(bigger number = more spawns per second)
    public float timeToReset = 5f; // Time after which an object is reset

    private List<GameObject> pooledObjects; // List practice because I need more list practice and make the script more flexible if you want to add variety n stuff
    private float nextSpawnTime = 0f;

    void Awake()
    {
        pooledObjects = new List<GameObject>();
        // Creating and storing the number in the pool
        for(int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false); 
            pooledObjects.Add(obj);
        }
    }

    void Update()
    {
        // Check if it's time to spawn another object
        if(Time.time >= nextSpawnTime)
        {
            SpawnSphere();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    // Spawns Sphere prefab
    void SpawnSphere()
    {
        GameObject sphere = GetPooledObject();
        if (sphere != null)
        {
            sphere.transform.position = this.transform.position;
            sphere.SetActive(true);
            StartCoroutine(ResetObject(sphere));
        }
    }

    //return object
    GameObject GetPooledObject()
    {
        foreach (var pooledObject in pooledObjects)
        {
            // Check if the object is not active
            if (!pooledObject.activeInHierarchy)
            {
                return pooledObject;
            }
        }
        return null;
    }

    // Coroutine to reset the object after a delay
    System.Collections.IEnumerator ResetObject(GameObject obj)
    {
        yield return new WaitForSeconds(timeToReset);
        obj.SetActive(false);
    }
}
