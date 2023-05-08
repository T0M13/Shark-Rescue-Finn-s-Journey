using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int amount;
    [SerializeField] protected List<GameObject> pooledObjects = new List<GameObject>();

    //spawnInterval should always be above 1
    [SerializeField] protected float spawnInterval = 1f;
    [SerializeField] protected int numOfRowObjects = 3;
    [SerializeField] protected float laneXDistance = 4f;
    [SerializeField] protected float laneYDistance = 3f;
    [Range(3, 3)]
    [SerializeField] protected int lanes = 3;
    protected Vector3 right = Vector3.right;
    protected Vector3 up = Vector3.up;
    private float timeSinceLastSpawn = 0f;
    [SerializeField] protected float rowDistance = 2f;
    [SerializeField] protected int numObjectsMin = 3;
    [SerializeField] protected int numObjectsMax = 5;
    private int lastXLaneIndex;
    private int lastYLaneIndex;

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
        if (timeSinceLastSpawn >= numOfRowObjects * spawnInterval)
        {
            SpawnRowOfObjects();
            timeSinceLastSpawn = 0f;
        }
        else
        {
            timeSinceLastSpawn += Time.deltaTime;
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

    public void SpawnRowOfObjects()
    {
        int laneXIndex = Random.Range(0, lanes);

        while (laneXIndex == lastXLaneIndex)
        {
            laneXIndex = Random.Range(0, lanes);
        }
        lastXLaneIndex = laneXIndex;


        int laneYIndex = Random.Range(0, lanes);

        while (laneYIndex == lastYLaneIndex)
        {
            laneYIndex = Random.Range(0, lanes);
        }
        lastYLaneIndex = laneYIndex;

         numOfRowObjects = Random.Range(numObjectsMin, numObjectsMax);

        for (int i = 0; i < numOfRowObjects; i++)
        {
            Vector3 pos = transform.position + (-laneXDistance + laneXDistance * laneXIndex) * right + (-laneYDistance + laneYDistance * laneYIndex) * up + Vector3.forward * (i * rowDistance);
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
