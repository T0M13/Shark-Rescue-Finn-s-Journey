using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkEnvironment : Chunk
{

    //void Update()
    //{
    //    base.Update();
    //}

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit " + other.gameObject.name);

        if (other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            //Debug.Log(gameObject.name + ": " + gameObject.transform.position);
            gameObject.SetActive(false);

            for (int i = 0; i < ChunkManager.Instance.chunkListEnvironment.Count; i++)
            {
                if (ChunkManager.Instance.chunkListEnvironment[i].EEnvironmentType == eEnvironmentType)
                {
                    ChunkManager.Instance.chunkListEnvironment[i].DisabledEnvChunkList.Add(gameObject);
                    ChunkManager.Instance.chunkListEnvironment[i].ActiveEnvChunkList.Remove(gameObject);
                }
            }
        }
    }

}
