using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk List")]
    public List<GameObject> chunks = new List<GameObject>();
    public List<GameObject> disabledChunks = new List<GameObject>();
    [SerializeField] private GameObject chunkColliderCatcher;
    
    [Header("Chunk Settings")]
    [SerializeField] private Vector3 chunkColliderCatcherSize = new Vector3(13, 15, 13);
    [SerializeField, Range(1, 20), Tooltip("It must alway be 2 less than the 'Chunk.Count'")] 
    private int maxChunksShownAtTime = 3;
    [SerializeField] private float chunksMovingSpeed;
    private int chunklength = 0;
    private int chunkwidth = 0;
    public float spawnAdjustment = 0f; //OnTriggerExit is not precisly enoguh (0.51;0.22;1.04) -> Difference needs to be added

    public static ChunkManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (chunks.Count == 0)
            return;

        chunklength = CreateChunks.Instance.chunklength;
        chunkwidth = CreateChunks.Instance.chunkwidth;

        CheckValues();
        AdjustChunkColliderCatcher();
        AdjustAllChunks();
    }

    private void AdjustChunkColliderCatcher() //At the beginning, the Chunk/Obstacle ColliderCatcher will be adjusted
    {
        chunkColliderCatcher.GetComponent<BoxCollider>().size = new Vector3(chunkColliderCatcherSize.x * chunkwidth, chunkColliderCatcherSize.y, chunkColliderCatcherSize.z);
        chunkColliderCatcher.transform.position = new Vector3(0, 0, -13 * (chunklength - 1));
    }

    private void AdjustAllChunks()
    {
        chunksMovingSpeed = GameManager.instance.GameSpeed;

        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;

            if (i > maxChunksShownAtTime - 1)
            {
                chunks[i].SetActive(false);
                disabledChunks.Add(chunks[i]);
                //Debug.Log(chunks[i] + " is SetActive false");
            }
        }

        for (int j = 0; j < maxChunksShownAtTime; j++)
        {
            chunks[j].transform.position = new Vector3(0, 0, 13 * chunklength * j);
        }
    }

    public void AdjustMovementSpeed()
    {
        if(GameManager.instance == null)
            return;

        chunksMovingSpeed = GameManager.instance.GameSpeed;

        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].GetComponent<Chunk>().movingSpeed = chunksMovingSpeed;
        }
    }

    public void AddNewChunk()
    {
        int randomTemp = Random.Range(0, disabledChunks.Count);
        //int distancMultipl;

        //if(maxChunksShownAtTime - 1 <= 0)
        //{
        //    distancMultipl = 1;
        //}
        //else
        //{
        //    //distancMultipl = maxChunksShownAtTime - 1;
        //    distancMultipl = maxChunksShownAtTime + 1;
        //}


        disabledChunks[randomTemp].transform.position = new Vector3(0, 0, 13 * chunklength * (maxChunksShownAtTime + 1) + spawnAdjustment);
        disabledChunks[randomTemp].SetActive(true);
        disabledChunks.RemoveAt(randomTemp);
    }

    private void CheckValues()
    {
        if(maxChunksShownAtTime > chunks.Count -2)
        {
            maxChunksShownAtTime = chunks.Count -3;
            Debug.LogWarning("'maxChunksShownAtTime' must always be at least 2 less than the 'Chunk.Count'\nIncrease the chunkQuantity for more 'maxChunksShownAtTime'");
        }
    }
}
