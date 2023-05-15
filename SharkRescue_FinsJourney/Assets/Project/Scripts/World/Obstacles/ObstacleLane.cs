using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLane : MonoBehaviour
{
    public float movementSpeed = 10;
    public List<Obstacle> obstacles;

    void Update()
    {
        gameObject.transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
    }




    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit " + other.gameObject.name);

        if (other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            ObstacleManager.Instance.allActiveObstaclesCounter--;
            
            for (int i = 0; i < obstacles.Count; i++) //Move obstacles from this lane to obstacleContainer
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                gameObject.transform.GetChild(0).gameObject.transform.parent = ObstacleManager.Instance.obstacleContainer.transform;

            }
            gameObject.SetActive(false);
            

            if (ObstacleManager.Instance.distanceAdjustment == 0) //After the first obstacle has been deactivated, the new obstacle spawns accordingly at the new location 
            {
                ObstacleManager.Instance.distanceAdjustment = 1;
            }


            for (int i = 0; i < ObstacleManager.Instance.ObstaclePrefabs.Count; i++)
            {
                for (int j = obstacles.Count - 1; j >= 0; j--)
                {
                    if (ObstacleManager.Instance.ObstaclePrefabs[i].obstacleType == obstacles[j].obstacleType)
                    {
                        ObstacleManager.Instance.ObstaclePrefabs[i].disabledObstacleList.Add(obstacles[j].gameObject);
                        ObstacleManager.Instance.ObstaclePrefabs[i].activeObstacleList.Remove(obstacles[j].gameObject);
                    }
                }
            }
            obstacles.Clear();
            ObstacleManager.Instance.disabledObstacleLanes.Add(gameObject);
            ObstacleManager.Instance.spawnAdjustment = gameObject.transform.position.x - 65;

            ObstacleManager.Instance.SpawnObstacles();
        }
    }
}
