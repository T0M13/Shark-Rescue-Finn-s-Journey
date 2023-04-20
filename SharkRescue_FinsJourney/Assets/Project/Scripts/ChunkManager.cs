using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunks = new List<GameObject>();
    [SerializeField] private int maxChunksShownAtTime = 3;
    [SerializeField] private int ChunksMovingSpeed = 5;
    [SerializeField] private CreateChunks createChunks;

    void Start()
    {
        if (chunks.Count == 0)
            return;


        int chunklength = createChunks.chunklength;


        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].GetComponent<Chunk>().MovingSpeed = ChunksMovingSpeed;
            if (i > maxChunksShownAtTime-1)
            {
                chunks[i].SetActive(false);
                Debug.Log(chunks[i] + " is SetActive false");
            }
        }

        for (int j = 0; j <= maxChunksShownAtTime; j++)
        {
            if (j < maxChunksShownAtTime)
            {
                chunks[j].transform.position = new Vector3(-13 * chunklength * j, 0, 0);
            }
        }
    }


}
