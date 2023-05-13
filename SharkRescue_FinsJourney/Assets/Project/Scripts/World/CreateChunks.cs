using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateChunks : MonoBehaviour
{
    public List<GameObject> mainChunksPref = new List<GameObject>();
    public List<GameObject> environmentChunksPref = new List<GameObject>();
    [Range(1, 20), Tooltip("How long should a Chunk be generated.")]
    public int chunklength = 3;
    [Range(3, 20), Tooltip("How wide should a Chunk be generated.\nNumber should be always odd (or it will add 1 number to it).")]
    public int chunkwidth = 3;
    [Range(1, 20), Tooltip("How many Chunks should be generated.")]
    public int chunkQuantity = 8;

    [SerializeField] private double tileWidthLength = 13.667138d;

    public static CreateChunks Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        CheckValues();
        CreateChunk();
    }

    private void CreateChunk()
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

            //Erstellung eines Chunks
            for (int i = 0; i < chunklength; i++)
            {
                var firstchunkwidth = Math.Ceiling(chunkwidth * 0.5);

                //Mid (inclusive) to Left
                for (int j = 0; j < firstchunkwidth; j++)
                {
                    if (i == 0)
                    {
                        int temp = Random.Range(0, mainChunksPref.Count);
                        //Debug.Log("Math.Ceiling(chunkwidth * 0.5) " + firstchunkwidth);
                        GameObject go = Instantiate(mainChunksPref[temp], transform.position, transform.rotation);
                        go.transform.position = new Vector3((float)-tileWidthLength * i, 0, (float)-tileWidthLength * j);
                        go.transform.parent = chunk.transform;
                    }
                    else
                    {
                        int temp = Random.Range(0, environmentChunksPref.Count);
                        GameObject go = Instantiate(environmentChunksPref[temp], transform.position, transform.rotation);
                        go.transform.position = new Vector3((float)-tileWidthLength * i, 0, (float)-tileWidthLength * j);
                        go.transform.parent = chunk.transform;
                    }
                }

                //Mid (exclusive) to Right
                for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
                {
                    int temp = Random.Range(0, environmentChunksPref.Count);
                    //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                    GameObject go = Instantiate(environmentChunksPref[temp]);
                    go.transform.position = new Vector3((float)-tileWidthLength * i, 0, (float)tileWidthLength * k);
                    go.transform.parent = chunk.transform;
                }
            }
            chunk.AddComponent<Chunk>();
            ChunkManager.Instance.chunks.Add(chunk);

            //Box Collider Settings
            chunk.AddComponent<BoxCollider>();
            chunk.GetComponent<BoxCollider>().isTrigger = true;
            //chunk.GetComponent<BoxCollider>().center = new Vector3(-13 * (chunklength-1),0,0);
            chunk.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
            chunk.GetComponent<BoxCollider>().size = new Vector3((float)tileWidthLength, 1, (float)tileWidthLength);
            chunk.transform.parent = chunksparent.transform;

            counter++;
        } while (counter < chunkQuantity);

    }

    private void CheckValues()
    {
        if (chunkwidth % 2 == 0)
        {
            chunkwidth++;
        }
        //Debug.Log("chunkwidth" + chunkwidth);
    }
}
