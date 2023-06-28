using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnvironmentType;

public class Chunk : MonoBehaviour
{
    public float movingSpeed = 5;
    public EEnvironmentType eEnvironmentType;

    void Update()
    {
        //gameObject.transform.position += new Vector3(movingSpeed * Time.deltaTime, 0, 0);
        gameObject.transform.position += new Vector3(0, 0, -movingSpeed * Time.deltaTime);

        //if(gameObject.transform.position.x >= 65)
        //{
        //    Debug.Log(gameObject.name + " Update: " + gameObject.transform.position);
        //}
    }


    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit " + other.gameObject.name);

        if(other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            //Debug.Log(gameObject.name + ": " + gameObject.transform.position);
            ChunkManager.Instance.spawnAdjustment = gameObject.transform.position.z + 65;
            gameObject.SetActive(false);
            ChunkManager.Instance.SpawnNewChunk();

            for (int i = 0; i < ChunkManager.Instance.chunkListEnvironment.Count; i++)
            {
                if (ChunkManager.Instance.chunkListEnvironment[i].EEnvironmentType == eEnvironmentType)
                {
                    ChunkManager.Instance.chunkListEnvironment[i].DisabledChunkList.Add(gameObject);
                    ChunkManager.Instance.chunkListEnvironment[i].ActiveChunkList.Remove(gameObject);
                }
            }
        }
    }
}
