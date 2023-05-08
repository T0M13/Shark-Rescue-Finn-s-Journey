using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] public ObstacleTypes.ObstacleType obstacleType;
    public float movingSpeed = 5;

    void Update()
    {
        gameObject.transform.position += new Vector3(movingSpeed * Time.deltaTime, 0, 0);
    }


    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit " + other.gameObject.name);

        if (other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            gameObject.SetActive(false);
            ObstacleManager.Instance.AddNewObstacle();
            ObstacleManager.Instance.disabledObstacles.Add(gameObject);
        }
    }
}
