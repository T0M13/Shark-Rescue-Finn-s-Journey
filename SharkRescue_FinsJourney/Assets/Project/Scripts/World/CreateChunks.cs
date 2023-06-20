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
    [Range(4, 20), Tooltip("How many Chunks should be generated.\nIt must alway be 2 more than the 'maxChunksShownAtTime' (ChunkManager) due to variation")]
    public int chunkQuantity = 8;

    [SerializeField] private float tileWidthLength = 13f;

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

        //StartCoroutine("CreateChunkTime");

        CreateChunk();
    }

    private void CreateChunk()
    {
        int counter = 0;
        //Chunk-Parent where all chunks are stored
        GameObject chunksparent = new GameObject();
        chunksparent.name = "ChunksParent";
        chunksparent.transform.position = new Vector3(0, 0, 0);

        do
        {
            //Debug.Log("Creating Chunk...");
            //Settings for a Chunk
            GameObject chunk = new GameObject();
            chunk.name = "chunk_0" + counter;
            chunk.transform.position = new Vector3(0, 0, 0);

            var firstchunkwidth = Math.Ceiling(chunkwidth * 0.5);
            //Debug.Log("Math.Ceiling(chunkwidth * 0.5) " + firstchunkwidth);
            //Creating a Chunk
            for (int i = 0; i < chunklength; i++) //Current Row
            {

                //Mid (inclusive) to Left
                for (int j = 0; j < firstchunkwidth; j++)
                {
                    if (i == 0)
                    {
                        int temp = Random.Range(0, mainChunksPref.Count);
                        GameObject go = Instantiate(mainChunksPref[temp], transform.position, transform.rotation);
                        go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                        go.transform.parent = chunk.transform;
                    }
                    else
                    {
                        int temp = Random.Range(0, environmentChunksPref.Count);
                        GameObject go = Instantiate(environmentChunksPref[temp], transform.position, transform.rotation);
                        go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                        go.transform.parent = chunk.transform;
                    }
                }

                //Mid (exclusive) to Right
                for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
                {
                    int temp = Random.Range(0, environmentChunksPref.Count);
                    //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                    GameObject go = Instantiate(environmentChunksPref[temp]);
                    go.transform.position = new Vector3(tileWidthLength * k, 0, tileWidthLength * i);
                    go.transform.parent = chunk.transform;
                }
            }
            chunk.AddComponent<Chunk>();
            ChunkManager.Instance.chunks.Add(chunk);

            //Box Collider Settings
            chunk.AddComponent<BoxCollider>();
            chunk.GetComponent<BoxCollider>().isTrigger = true;
            chunk.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
            chunk.GetComponent<BoxCollider>().size = new Vector3(tileWidthLength, 1, tileWidthLength);
            chunk.transform.parent = chunksparent.transform;

            counter++;
        } while (counter < chunkQuantity);

    }

    private IEnumerator CreateChunkTime()
    {
        //yield return new WaitForSeconds(2f);

        int counter = 0;
        //Chunk-Parent where all chunks are stored
        GameObject chunksparent = new GameObject();
        chunksparent.name = "ChunksParent";
        chunksparent.transform.position = new Vector3(0, 0, 0);

        do
        {
            Debug.Log("Creating Chunk...");
            //Settings for a Chunk
            GameObject chunk = new GameObject();
            chunk.name = "chunk_0" + counter;
            chunk.transform.position = new Vector3(0, 0, 0);

            var firstchunkwidth = Math.Ceiling(chunkwidth * 0.5);
            Debug.Log("Math.Ceiling(chunkwidth * 0.5) " + firstchunkwidth);
            //Creating a Chunk
            for (int i = 0; i < chunklength; i++) //Current Row
            {

                //Mid (inclusive) to Left
                for (int j = 0; j < firstchunkwidth; j++)
                {
                    if (i == 0)
                    {
                        int temp = Random.Range(0, mainChunksPref.Count);
                        GameObject go = Instantiate(mainChunksPref[temp], transform.position, transform.rotation);
                        go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                        go.transform.parent = chunk.transform;
                    }
                    else
                    {
                        int temp = Random.Range(0, environmentChunksPref.Count);
                        GameObject go = Instantiate(environmentChunksPref[temp], transform.position, transform.rotation);
                        go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                        go.transform.parent = chunk.transform;
                    }


                    yield return new WaitForSeconds(0.75f);
                }

                //Mid (exclusive) to Right
                for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
                {
                    int temp = Random.Range(0, environmentChunksPref.Count);
                    //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                    GameObject go = Instantiate(environmentChunksPref[temp]);
                    go.transform.position = new Vector3(tileWidthLength * k, 0, tileWidthLength * i);
                    go.transform.parent = chunk.transform;


                    yield return new WaitForSeconds(0.75f);
                }
            }
            chunk.AddComponent<Chunk>();
            ChunkManager.Instance.chunks.Add(chunk);

            //Box Collider Settings
            chunk.AddComponent<BoxCollider>();
            chunk.GetComponent<BoxCollider>().isTrigger = true;
            chunk.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
            chunk.GetComponent<BoxCollider>().size = new Vector3(tileWidthLength, 1, tileWidthLength);
            chunk.transform.parent = chunksparent.transform;

            counter++;
        } while (counter < chunkQuantity);

    }

    /// <summary>
    /// Chunkwidth should always be odd (even -> odd (+1))
    /// </summary>
    private void CheckValues()
    {
        if (chunkwidth % 2 == 0)
        {
            chunkwidth++;
        }
        //Debug.Log("chunkwidth" + chunkwidth);
    }
}
