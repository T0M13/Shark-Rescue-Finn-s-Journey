using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnDistance : MonoBehaviour
{
    public GameObject spawnOb;

    public Vector3 PosDifference1 = new(4,2,0);
    public float spawnDifference1 = 3f;
    public int countDifference1 = 16;
    public Vector3 PosDifference2 = new(0,5,0);
    public float spawnDifference2 = 4f;
    public int countDifference2 = 12;
    public Vector3 PosDifference3 = new(-4,8,0);
    public float spawnDifference3 = 5f;
    public int countDifference3 = 12;

    void Start()
    {
        Spawn(PosDifference1 + new Vector3(0,0,65), spawnDifference3, 5);
        Spawn(PosDifference2 + new Vector3(0, 0, 65), spawnDifference3, 9);
        Spawn(PosDifference3 + new Vector3(0, 0, 65), spawnDifference3, 12);
    }

    private void Spawn(Vector3 pos, float spawn, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(spawnOb);
            go.transform.position = pos + new Vector3(0f,0f,6 + i * spawn);
        }
    }

}
