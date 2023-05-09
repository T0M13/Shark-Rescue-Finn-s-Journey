using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] public ObstacleTypes.ObstacleType obstacleType;
    public float movementSpeed = 5;

    void Update()
    {
        gameObject.transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
    }


    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit " + other.gameObject.name);

        if (other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            gameObject.SetActive(false);
            ObstacleManager.Instance.allActiveObstaclesCounter--;
            
            if(ObstacleManager.Instance.distanceAdjustment == 0) //After the first obstacle has been deactivated, the new obstacle spawns accordingly at location 
            {
                ObstacleManager.Instance.allActiveObstaclesCounter = 1;
            }

            ObstacleManager.Instance.SpawnObstacles();

            for (int i = 0; i < ObstacleManager.Instance.ObstaclePrefabs.Count; i++)
            {
                if (ObstacleManager.Instance.ObstaclePrefabs[i].obstacleType == obstacleType)
                {
                    ObstacleManager.Instance.ObstaclePrefabs[i].disabledObstacleList.Add(gameObject);
                    ObstacleManager.Instance.ObstaclePrefabs[i].activeObstacleList.Remove(gameObject);
                }
            }
        }
    }
}
