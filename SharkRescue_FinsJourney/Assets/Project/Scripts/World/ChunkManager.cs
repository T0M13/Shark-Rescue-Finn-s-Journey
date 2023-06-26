using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkTypes;
using Random = UnityEngine.Random;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Creator")]
    [Range(1, 20), Tooltip("How long should a Chunk be generated.")]
    public int chunklength = 5;
    [Range(3, 20), Tooltip("How wide should a Chunk be generated.\nNumber should be always odd (or it will add 1 number to it).")]
    public int chunkwidth = 5;
    [Range(4, 20), Tooltip("How many Chunks should be generated.\nIt must alway be 2 more than the 'maxChunksShownAtTime' (ChunkManager) due to variation")]
    public int chunkQuantity = 8;
    [SerializeField] private float tileWidthLength = 13f;

    [Header("Chunk Settings")]
    [SerializeField] private EChunkType eChunkType = new();
    [SerializeField] private Vector3 chunkColliderCatcherSize = new Vector3(13, 15, 13);
    [SerializeField, Range(1, 20), Tooltip("It must alway be 2 less than the 'Chunk.Count'")]
    private int maxChunksShownAtTime = 3;
    [SerializeField] private float chunksMovingSpeed;
    public float spawnAdjustment = 0f; //OnTriggerExit is not precisly enoguh (0.51;0.22;1.04) -> Difference needs to be added

    public EChunkType EChunkType { get => eChunkType; }

    [Header("Chunk List")]
    public List<ChunkListPrefabs> chunks = new();
    [SerializeField] private GameObject chunkColliderCatcher;


    public static ChunkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CheckValuesBeforeCreating();
        CreateChunk();

    }

    void Start()
    {

        AdjustChunkColliderCatcher();

        CheckValuesBeforeEnabling();
        SpawnFirstChunks();
    }

    private void CreateChunk()
    {
        int counter = 0;
        //Chunk-Parent where all chunks are stored
        GameObject chunksparent = new GameObject();
        chunksparent.name = "ChunksParent";
        chunksparent.transform.position = new Vector3(0, 0, 0);

        for (int h = 0; h < chunks.Count; h++)
        {

            while (counter < chunkQuantity)
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
                            int temp = Random.Range(0, chunks[h].MainFloorTilePrefabs.Count);
                            GameObject go = Instantiate(chunks[h].MainFloorTilePrefabs[temp], transform.position, transform.rotation);
                            go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                            go.transform.parent = chunk.transform;
                        }
                        else
                        {
                            int temp = Random.Range(0, chunks[h].EnvFloorTilePrefabs.Count);
                            GameObject go = Instantiate(chunks[h].EnvFloorTilePrefabs[temp], transform.position, transform.rotation);
                            go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                            go.transform.parent = chunk.transform;
                        }
                    }

                    //Mid (exclusive) to Right
                    for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
                    {
                        int temp = Random.Range(0, chunks[h].EnvFloorTilePrefabs.Count);
                        //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                        GameObject go = Instantiate(chunks[h].EnvFloorTilePrefabs[temp]);
                        go.transform.position = new Vector3(tileWidthLength * k, 0, tileWidthLength * i);
                        go.transform.parent = chunk.transform;
                    }
                }

                chunk.AddComponent<Chunk>();
                //Box Collider Settings
                chunk.AddComponent<BoxCollider>();
                chunk.GetComponent<BoxCollider>().isTrigger = true;
                chunk.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
                chunk.GetComponent<BoxCollider>().size = new Vector3(tileWidthLength, 1, tileWidthLength);
                chunk.SetActive(false);
                chunk.transform.parent = chunksparent.transform;
                chunk.GetComponent<Chunk>().eChunkType = chunks[h].EChunkType;

                chunks[h].DisabledChunkList.Add(chunk);

                counter++;
            };
        }


    }


    private void AdjustChunkColliderCatcher() //At the beginning, the Chunk/Obstacle ColliderCatcher will be adjusted
    {
        chunkColliderCatcher.GetComponent<BoxCollider>().size = new Vector3(chunkColliderCatcherSize.x * chunkwidth, chunkColliderCatcherSize.y, chunkColliderCatcherSize.z);
        chunkColliderCatcher.transform.position = new Vector3(0, 0, -tileWidthLength * (chunklength - 1));
    }

    public void AdjustMovementSpeed()
    {
        if (GameManager.instance == null)
            return;

        chunksMovingSpeed = GameManager.instance.GameSpeed;

        for (int i = 0; i < chunks.Count; i++)
        {
            for (int j = 0; j < chunks[i].DisabledChunkList.Count; j++)
            {
                chunks[i].DisabledChunkList[j].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;
            }

            for (int j = 0; j < chunks[i].ActiveChunkList.Count; j++)
            {
                chunks[i].ActiveChunkList[j].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;
            }
        }
    }

    public void ChangeEnvironment(EChunkType eChunkType)
    {
        if (GameManager.instance == null)
            return;

        this.eChunkType = eChunkType;
    }

    private void SpawnFirstChunks()
    {
        if (GameManager.instance == null)
            return;

        chunksMovingSpeed = GameManager.instance.GameSpeed;
        eChunkType = GameManager.instance.EChunkType;

        for (int i = 0; i < chunks.Count; i++)
        {
            for (int j = chunks[i].DisabledChunkList.Count - 1; j >= 0; j--)
            {
                chunks[i].DisabledChunkList[j].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;

                if (j < maxChunksShownAtTime && chunks[i].EChunkType == eChunkType)
                {
                    chunks[i].ActiveChunkList.Add(chunks[i].DisabledChunkList[j]);

                    chunks[i].DisabledChunkList[j].SetActive(true);
                    chunks[i].DisabledChunkList[j].transform.position = new Vector3(0, 0, 13 * chunklength * j);

                    chunks[i].DisabledChunkList.Remove(chunks[i].DisabledChunkList[j]);
                }
            }
        }
    }
    
    public void SpawnNewChunk()
    {
        int randomTemp;
        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i].EChunkType == eChunkType)
            {
                randomTemp = Random.Range(0, chunks[i].DisabledChunkList.Count);

                chunks[i].ActiveChunkList.Add(chunks[i].DisabledChunkList[randomTemp]);

                chunks[i].DisabledChunkList[randomTemp].transform.position = new Vector3(0, 0, tileWidthLength * chunklength * (maxChunksShownAtTime - 1) + spawnAdjustment);
                chunks[i].DisabledChunkList[randomTemp].SetActive(true);

                chunks[i].DisabledChunkList.RemoveAt(randomTemp);

            }
        }


        //disabledChunks[randomTemp].transform.position = new Vector3(0, 0, 13 * chunklength * (maxChunksShownAtTime + 1) + spawnAdjustment);
        //disabledChunks[randomTemp].SetActive(true);
        //disabledChunks.RemoveAt(randomTemp);
    }

    private void CheckValuesBeforeCreating()
    {
        if (chunkwidth % 2 == 0) //Chunkwidth should always be odd (even -> odd (+1))
        {
            chunkwidth++;
        }
        //Debug.Log("chunkwidth" + chunkwidth);
    }

    private void CheckValuesBeforeEnabling()
    {
        ///if(maxChunksShownAtTime > chunks.Count -2)
        ///{
        ///    maxChunksShownAtTime = chunks.Count -3;
        ///    Debug.LogWarning("'maxChunksShownAtTime' must always be at least 2 less than the 'Chunk.Count'\nIncrease the chunkQuantity for more 'maxChunksShownAtTime'");
        ///}
    }
}
