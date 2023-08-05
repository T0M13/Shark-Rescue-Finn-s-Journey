using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnvironmentType;
using Random = UnityEngine.Random;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Creator")]
    [Range(1, 20), Tooltip("How long should a chunk be generated.")]
    public int chunklength = 5;
    [Range(3, 20), Tooltip("How wide should a chunk be generated.\nNumber should be always odd (or it will add 1 number to it).")]
    public int chunkwidth = 5;
    [Range(4, 20), Tooltip("How many chunks should be generated.\nIt must always be 2 more than the 'maxChunksShownAtTime' (ChunkManager) due to variation")]
    public int chunkQuantity = 8;
    [Range(4, 20), Tooltip("How many environment chunks should be generated.\nIt must overall be 2 more than the 'maxChunksShownAtTime' (ChunkManager) due to variation")]
    public int environmentChunkQuantity = 8;
    [SerializeField] private float tileWidthLength = 13f;

    [Header("Chunk Settings")]
    [SerializeField] private EEnvironmentType eCurrentEnvironmentType = new();
    [SerializeField] private Vector3 chunkColliderCatcherSize = new Vector3(13, 15, 13);
    [SerializeField, Range(1, 20), Tooltip("It must alway be 2 less than the 'Chunk.Count'")]
    private int maxChunksShownAtTime = 3;
    [SerializeField] private float chunksMovingSpeed;
    public float spawnAdjustment = 0f; //OnTriggerExit is not precisly enoguh (0.51;0.22;1.04) -> Difference needs to be added

    [Header("Chunk List")]
    public List<ChunkListPrefabs> chunkListEnvironment = new();
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

        //CheckValuesBeforeEnabling();
        SpawnFirstChunks();
    }

    private void CreateChunk()
    {
        //Create Floor Chunks
        int counter;
        //Chunk-Parent where all chunks are stored
        GameObject mainChunksParent = new GameObject();
        mainChunksParent.name = "MainChunksParent";
        mainChunksParent.transform.position = new Vector3(0, 0, 0);

        for (int h = 0; h < chunkListEnvironment.Count; h++)
        {
            counter = 0;

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
                        if (j == 0)
                        {
                            int temp = Random.Range(0, chunkListEnvironment[h].MainFloorTilePrefabs.Count);
                            GameObject go = Instantiate(chunkListEnvironment[h].MainFloorTilePrefabs[temp], transform.position, transform.rotation);
                            go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                            go.transform.parent = chunk.transform;
                        }
                        else
                        {
                            int temp = Random.Range(0, chunkListEnvironment[h].EnvFloorTilePrefabs.Count);
                            GameObject go = Instantiate(chunkListEnvironment[h].EnvFloorTilePrefabs[temp], transform.position, transform.rotation);
                            go.transform.position = new Vector3(-tileWidthLength * j, 0, tileWidthLength * i);
                            go.transform.parent = chunk.transform;
                        }
                    }

                    //Mid (exclusive) to Right
                    for (int k = 1; k < chunkwidth + 1 - firstchunkwidth; k++)
                    {
                        int temp = Random.Range(0, chunkListEnvironment[h].EnvFloorTilePrefabs.Count);
                        //Debug.Log("Math.Floor(chunkwidth - chunkwidth * 0.5) " + Math.Floor(chunkwidth * 0.5));
                        GameObject go = Instantiate(chunkListEnvironment[h].EnvFloorTilePrefabs[temp]);
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
                chunk.transform.parent = mainChunksParent.transform;
                chunk.GetComponent<Chunk>().eEnvironmentType = chunkListEnvironment[h].EEnvironmentType;

                chunkListEnvironment[h].DisabledChunkList.Add(chunk);

                counter++;
            };
        }

        //Dublicate Environment Chunks
        GameObject envChunksParent = new GameObject();
        envChunksParent.name = "EnvChunksParent";
        envChunksParent.transform.position = new Vector3(0, 0, 0);


        for (int i = 0; i < chunkListEnvironment.Count; i++)
        {
            for (int j = 0; j < chunkListEnvironment[i].EnvChunkPrefabs.Count; j++)
            {
                for (int k = 0; k < environmentChunkQuantity; k++)//How often should they be replicated
                {
                    GameObject go = Instantiate(chunkListEnvironment[i].EnvChunkPrefabs[j], transform.position, transform.rotation);
                    go.transform.parent = envChunksParent.transform;
                    go.SetActive(false);
                    //go.transform.position = new Vector3(0,0,0);
                    chunkListEnvironment[i].DisabledEnvChunkList.Add(go);
                }
            }
        }

    }


    private void AdjustChunkColliderCatcher() //At the beginning, the Chunk/Obstacle ColliderCatcher will be adjusted
    {
        chunkColliderCatcher.GetComponent<BoxCollider>().size = new Vector3(chunkColliderCatcherSize.x * chunkwidth, chunkColliderCatcherSize.y, chunkColliderCatcherSize.z);
        chunkColliderCatcher.transform.position = new Vector3(0, 0, -tileWidthLength * (chunklength - 1));
    }

    public void AdjustMovementSpeed()
    {
        if (GameManager.Instance == null)
            return;

        chunksMovingSpeed = GameManager.Instance.GameSpeed;

        for (int i = 0; i < chunkListEnvironment.Count; i++)
        {
            //Disabled floor chunks
            for (int j = 0; j < chunkListEnvironment[i].DisabledChunkList.Count; j++)
            {
                chunkListEnvironment[i].DisabledChunkList[j].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;
            }

            //Active floor chunks
            for (int j = 0; j < chunkListEnvironment[i].ActiveChunkList.Count; j++)
            {
                chunkListEnvironment[i].ActiveChunkList[j].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;
            }

            //Disabled environment chunks
            for (int j = 0; j < chunkListEnvironment[i].DisabledEnvChunkList.Count; j++)
            {
                chunkListEnvironment[i].DisabledEnvChunkList[j].GetComponent<ChunkEnvironment>().movingSpeed = chunksMovingSpeed;
            }

            //Active environment chunks
            for (int j = 0; j < chunkListEnvironment[i].ActiveEnvChunkList.Count; j++)
            {
                chunkListEnvironment[i].ActiveEnvChunkList[j].GetComponent<ChunkEnvironment>().movingSpeed = chunksMovingSpeed;
            }
        }
    }

    public void ChangeEnvironment(EEnvironmentType eChunkType)
    {
        if (GameManager.Instance == null)
            return;

        this.eCurrentEnvironmentType = eChunkType;
    }

    private void SpawnFirstChunks()
    {
        if (GameManager.Instance == null)
            return;

        chunksMovingSpeed = GameManager.Instance.GameSpeed;
        eCurrentEnvironmentType = GameManager.Instance.EEnvironmentTyp;

        //Debug.Log("eCurrentEnvironmentType " + eCurrentEnvironmentType);

        for (int i = 0; i < chunkListEnvironment.Count; i++)
        {

            for (int j = chunkListEnvironment[i].DisabledChunkList.Count - 1; j >= 0; j--)
            {
                chunkListEnvironment[i].DisabledChunkList[j].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;

                if (j < maxChunksShownAtTime && chunkListEnvironment[i].EEnvironmentType == eCurrentEnvironmentType)
                {
                    chunkListEnvironment[i].ActiveChunkList.Add(chunkListEnvironment[i].DisabledChunkList[j]);

                    chunkListEnvironment[i].DisabledChunkList[j].SetActive(true);
                    chunkListEnvironment[i].DisabledChunkList[j].transform.position = new Vector3(0, 0, 13 * chunklength * j);

                    chunkListEnvironment[i].DisabledChunkList.Remove(chunkListEnvironment[i].DisabledChunkList[j]);
                }
            }

            for (int j = chunkListEnvironment[i].DisabledEnvChunkList.Count - 1; j >= 0; j--)
            {
                chunkListEnvironment[i].DisabledEnvChunkList[j].GetComponent<ChunkEnvironment>().movingSpeed = chunksMovingSpeed;

                if (j < maxChunksShownAtTime && chunkListEnvironment[i].EEnvironmentType == eCurrentEnvironmentType)
                {
                    chunkListEnvironment[i].ActiveEnvChunkList.Add(chunkListEnvironment[i].DisabledEnvChunkList[j]);

                    chunkListEnvironment[i].DisabledEnvChunkList[j].SetActive(true);
                    chunkListEnvironment[i].DisabledEnvChunkList[j].transform.position = new Vector3(0, 0, 13 * chunklength * j);

                    chunkListEnvironment[i].DisabledEnvChunkList.Remove(chunkListEnvironment[i].DisabledEnvChunkList[j]);
                }
            }



        }
    }

    public void SpawnNewChunk()
    {
        int randomTemp;

        eCurrentEnvironmentType = GameManager.Instance.EEnvironmentTyp;

        for (int i = 0; i < chunkListEnvironment.Count; i++)
        {
            if (chunkListEnvironment[i].EEnvironmentType == eCurrentEnvironmentType)
            {
                //Spawning floor chunk
                randomTemp = Random.Range(0, chunkListEnvironment[i].DisabledChunkList.Count);
                //Debug.Log("randomTemp " + randomTemp);

                chunkListEnvironment[i].ActiveChunkList.Add(chunkListEnvironment[i].DisabledChunkList[randomTemp]);

                chunkListEnvironment[i].DisabledChunkList[randomTemp].transform.position = new Vector3(0, 0, tileWidthLength * chunklength * (maxChunksShownAtTime - 1) + spawnAdjustment);
                chunkListEnvironment[i].DisabledChunkList[randomTemp].SetActive(true);

                chunkListEnvironment[i].DisabledChunkList.RemoveAt(randomTemp);


                //Adding the environment to the floor chunk
                randomTemp = Random.Range(0, chunkListEnvironment[i].DisabledEnvChunkList.Count);

                chunkListEnvironment[i].ActiveEnvChunkList.Add(chunkListEnvironment[i].DisabledEnvChunkList[randomTemp]);

                chunkListEnvironment[i].DisabledEnvChunkList[randomTemp].transform.position = new Vector3(0, 0, tileWidthLength * chunklength * (maxChunksShownAtTime - 1) + spawnAdjustment);
                chunkListEnvironment[i].DisabledEnvChunkList[randomTemp].SetActive(true);

                chunkListEnvironment[i].DisabledEnvChunkList.RemoveAt(randomTemp);
                
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
