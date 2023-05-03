using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int amount;
    [SerializeField] protected List<GameObject> pooledObjects = new List<GameObject>();
    [SerializeField] protected float minSpawnInterval = 1f;
    [SerializeField] protected float maxSpawnInterval = 3f;
    [SerializeField] protected float laneXDistance = 4f;
    [SerializeField] protected float laneYDistance = 3f;
    [Range(0, 3)]
    [SerializeField] protected int lanes = 3;
    protected Vector3 right = Vector3.right;
    protected Vector3 up = Vector3.up;
    protected float timeSinceLastSpawn = 0f;
    [SerializeField] protected bool rowSpawning = true;
    [SerializeField] protected float rowDistance = 2f;
    [SerializeField] protected int minRowObjectAmount = 3;
    [SerializeField] protected int maxRowObjectAmount = 5;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;

    protected virtual void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject clone = Instantiate(prefab, transform);
            clone.SetActive(false);
            pooledObjects.Add(clone);
        }
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= Random.Range(minSpawnInterval, maxSpawnInterval))
        {
            if (!rowSpawning)
                SpawnObject(GetSpawnPosition());
            else
                SpawnRowOfObjects(rowDistance, Random.Range(minRowObjectAmount, maxRowObjectAmount));
            timeSinceLastSpawn = 0f;
        }
    }

    protected virtual void SpawnObject(Vector3 position)
    {
        GameObject obj = GetAPooledObject();
        if (obj != null)
        {
            Vector3 pos = position;
            obj.transform.position = pos;
            obj.SetActive(true);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        int maxAttempts = 5;
        int currentAttempt = 0;
        bool positionFound = false;
        Vector3 pos = Vector3.zero;

        while (!positionFound && currentAttempt < maxAttempts)
        {
            currentAttempt++;
            pos = transform.position + (-laneXDistance + laneXDistance * Random.Range(0, lanes)) * right + (-laneYDistance + laneYDistance * Random.Range(0, lanes)) * up;

            Collider[] colliders = Physics.OverlapSphere(pos, 0.5f);
            if (colliders.Length == 0)
            {
                positionFound = true;
            }
        }

        if (!positionFound)
        {
            Debug.LogWarning("Unable to find a suitable spawn position after " + maxAttempts + " attempts.");
        }

        return pos;
    }

    public void SpawnRowOfObjects(float distanceBetweenObjects, int numObjects)
    {
        int laneXIndex = Random.Range(0, lanes);
        int laneYIndex = Random.Range(0, lanes);

        for (int i = 0; i < numObjects; i++)
        {
            Vector3 pos = transform.position + (-laneXDistance + laneXDistance * laneXIndex) * right + (-laneYDistance + laneYDistance * laneYIndex) * up + Vector3.forward * (i * distanceBetweenObjects);
            SpawnObject(pos);

        }
    }

    public void DeactivateObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public GameObject AddGameObject()
    {
        GameObject clone = Instantiate(prefab, transform);
        clone.SetActive(false);
        pooledObjects.Add(clone);
        amount++;
        return clone;
    }

    public GameObject GetAPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (AllObjectsDisabled)
        {
            return AddGameObject();
        }

        return null;
    }

    public bool AllObjectsDisabled
    {
        get
        {
            return ObjectsCheckDisabled();
        }
    }

    private bool ObjectsCheckDisabled()
    {
        bool check = pooledObjects.All(b => b.gameObject.activeSelf == false);
        if (check)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.magenta;

        for (int i = 0; i < lanes; i++)
        {
            for (int j = 0; j < lanes; j++)
            {
                Vector3 pos = transform.position + (-laneXDistance + laneXDistance * j) * right + (-laneYDistance + laneYDistance * i) * up;
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }
    }
}
