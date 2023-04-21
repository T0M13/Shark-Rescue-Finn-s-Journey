using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public float MovingSpeed = 5;
    
    void Update()
    {
        gameObject.transform.position += new Vector3(MovingSpeed * Time.deltaTime, 0, 0);
    }


    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit " + other.gameObject.name);

        if(other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            gameObject.SetActive(false);
            ChunkManager.instance.AddNewChunk();
            ChunkManager.instance.disabledChunks.Add(gameObject);
        }
    }
}
