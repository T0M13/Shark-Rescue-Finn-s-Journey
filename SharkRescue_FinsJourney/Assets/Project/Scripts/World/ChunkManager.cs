using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunks = new List<GameObject>();
    public List<GameObject> disabledChunks = new List<GameObject>();
    [SerializeField] private GameObject chunkColliderCatcher;
    [SerializeField] private Vector3 chunkColliderCatcherSize = new Vector3(13, 15, 13);
    [Range(1, 20)]
    [SerializeField] private int maxChunksShownAtTime = 3;
    [SerializeField] private int ChunksMovingSpeed = 5;
    [SerializeField] private CreateChunks createChunks;
    private int chunklength = 0;

    public static ChunkManager instance { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
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

        chunklength = createChunks.chunklength;
        
        chunkColliderCatcher.GetComponent<BoxCollider>().size = new Vector3(chunkColliderCatcherSize.x, chunkColliderCatcherSize.y, chunkColliderCatcherSize.z * createChunks.chunkwidth);
        chunkColliderCatcher.gameObject.transform.position = new Vector3(13 * (chunklength -1), 0,0);



        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].GetComponent<Chunk>().MovingSpeed = ChunksMovingSpeed;
            
            if (i >= maxChunksShownAtTime-1)
            {
                chunks[i].SetActive(false);
                disabledChunks.Add(chunks[i]);
                //Debug.Log(chunks[i] + " is SetActive false");
            }
        }

        for (int j = 0; j < maxChunksShownAtTime; j++)
        {
           chunks[j].transform.position = new Vector3(-13 * chunklength * j, 0, 0);
        }
    }

    public void AddNewChunk()
    {
        int temp = Random.Range(0, disabledChunks.Count);
        disabledChunks[temp].gameObject.transform.position = new Vector3(-13 * chunklength * (maxChunksShownAtTime - 2), 0, 0);
        disabledChunks[temp].gameObject.SetActive(true);
        disabledChunks.RemoveAt(temp);
    }
}
