using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateChunks : MonoBehaviour
{
    [SerializeField] private ChunkManager chunkManager;
    public List<GameObject> chunks = new List<GameObject>();
    [Range(1, 20)]
    public int chunklength = 3;
    [Range(1, 20)]
    public int chunkwidth = 3;
    [Range(1, 20)]
    public int chunkQuantity = 7;


    void Awake()
    {
        int counter = 0;
        //Settigs für eine Chunk-Parent
        GameObject chunksparent = new GameObject();
        chunksparent.name = "ChunksParent";
        chunksparent.transform.position = new Vector3(0, 0, 0);
        
        do
        {
            //Settings für ein Chunk
            GameObject chunk = new GameObject();
            chunk.name = "chunk_0" + counter;
            chunk.transform.position = new Vector3(0, 0, 0);
            chunk.tag = "Chunk";

            //Erstellung eines Chunks
            for (int i = 0; i < chunklength; i++)
            {
                var firstchunkwidth = Math.Ceiling(chunkwidth * 0.5);
                //Mid (inclusive) to Left
                for (int j = 0; j < firstchunkwidth; j++)
                {
                    int temp = Random.Range(0, chunks.Count);
                    //Debug.Log("Math.Ceiling(chunkwidth * 0.5) " + firstchunkwidth);
                    GameObject go = Instantiate(chunks[temp], transform.position, transform.rotation);
                    go.transform.position = new Vector3(-13 * i, 0, -13 * j);
                    go.transform.parent = chunk.transform;
                }
                //Mid (exclusive) to Right
                for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
                {
                    int temp = Random.Range(0, chunks.Count);
                    //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                    GameObject go = Instantiate(chunks[temp]);
                    go.transform.position = new Vector3(-13 * i, 0, 13 * k);
                    go.transform.parent = chunk.transform;
                }
            }
            chunk.AddComponent<Chunk>();
            chunkManager.chunks.Add(chunk);
            
            //Box Collider Settings
            chunk.AddComponent<BoxCollider>();
            //chunk.GetComponent<BoxCollider>().isTrigger = true;
            //chunk.GetComponent<BoxCollider>().center = new Vector3(-13 * (chunklength-1),0,0);
            chunk.GetComponent<BoxCollider>().center = new Vector3(0,0,0);
            chunk.GetComponent<BoxCollider>().size = new Vector3(13 , 1 , 13 );
            chunk.transform.parent = chunksparent.transform;

            counter++;
        } while (counter < chunkQuantity);
    }

}
