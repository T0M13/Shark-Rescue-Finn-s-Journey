using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] protected ItemScriptable[] prefabVariants;
    [SerializeField] protected int amount;
    [SerializeField] protected List<GameObject> pooledList = new List<GameObject>();
    [SerializeField] protected List<float> pooledListChances = new List<float>();

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
        for (int i = 0; i < prefabVariants.Length; i++)
        {
            for (int j = 0; j < amount; j++)
            {
                GameObject clone = Instantiate(prefabVariants[i].prefab, transform);
                clone.SetActive(false);
                pooledList.Add(clone);
                pooledListChances.Add(prefabVariants[i].chanceToSpawn);
            }
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

    public GameObject GetAPooledObject()
    {
        int randomIndex = Random.Range(0, pooledList.Count);
        if (!pooledList[randomIndex].activeInHierarchy)
        {
            if (pooledListChances[randomIndex] > Random.value)
                return pooledList[randomIndex];
            else
                return GetAPooledObject();
        }
        else
        {
            return GetAPooledObject();
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

