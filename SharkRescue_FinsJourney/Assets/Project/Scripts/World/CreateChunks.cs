using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateChunks : MonoBehaviour
{
    public List<GameObject> chunks = new List<GameObject>();
    public int chunklength = 3;
    public int chunkwidth = 3;


    void Start()
    {
        for (int i = 0; i < chunklength; i++)
        {
            var firstchunkwidth = Math.Ceiling(chunkwidth * 0.5);
            //Mid (inclusive) to Left
            for (int j = 0; j < firstchunkwidth; j++)
            {
                int temp = Random.Range(0, chunks.Count);
                //Debug.Log("Math.Ceiling(chunkwidth * 0.5) " + firstchunkwidth);
                GameObject go = Instantiate(chunks[temp],transform.position, transform.rotation);
                go.transform.position = new Vector3(-13 * i, 0, -13 * j);
            }
            //Mid (exclusive) to Right
            for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
            {
                int temp = Random.Range(0, chunks.Count);
                //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                GameObject go = Instantiate(chunks[temp]);
                go.transform.position = new Vector3(-13 * i, 0, 13 * k);
            }
        }
    }

}
