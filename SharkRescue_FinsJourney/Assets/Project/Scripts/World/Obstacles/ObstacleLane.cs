using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnvironmentType;

public class ObstacleLane : MonoBehaviour
{
    public float movementSpeed = 10;
    public List<Obstacle> obstacles;

    public EEnvironmentType eCurrentEnvironmentType;


    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, -movementSpeed * Time.deltaTime);
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


            //if (ObstacleManager.Instance.distanceAdjustment == 0) //After the first obstacle has been deactivated, the new obstacle spawns accordingly at the new location 
            //{
            //    ObstacleManager.Instance.distanceAdjustment = 1;
            //}


            for (int i = 0; i < ObstacleManager.Instance.ObstacleList.Count; i++)
            {
                if (ObstacleManager.Instance.ObstacleList[i].environmentType == eCurrentEnvironmentType)
                {
                    for (int j = 0; j < ObstacleManager.Instance.ObstacleList[i].obstacleConfigurations.Count; j++)
                    {
                        for (int k = obstacles.Count - 1; k >= 0; k--)
                        {
                            if (ObstacleManager.Instance.ObstacleList[i].obstacleConfigurations[j].obstacleType == obstacles[k].obstacleSizeType)
                            {
                                ObstacleManager.Instance.ObstacleList[i].obstacleConfigurations[j].disabledObstacleList.Add(obstacles[k].gameObject);
                                ObstacleManager.Instance.ObstacleList[i].obstacleConfigurations[j].activeObstacleList.Remove(obstacles[k].gameObject);
                            }
                        }
                    }
                }

            }
            obstacles.Clear();
            eCurrentEnvironmentType = EEnvironmentType.NONE;
            ObstacleManager.Instance.disabledObstacleLanes.Add(gameObject);
            ObstacleManager.Instance.spawnAdjustment = gameObject.transform.position.x - 65;

            ObstacleManager.Instance.SpawnObstacles();
        }
    }
}
