using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObstacles : MonoBehaviour
{
    public List<GameObject> obstaclesPref = new List<GameObject>();
    public List<GameObject> obstacles = new List<GameObject>();
    [Range(1, 20), Tooltip("How many of each obstacle should be generated.")]
    public int obstacleQuantity = 3;
    

    void Start()
    {
        CreateObstacle();
    }

    private void CreateObstacle()
    {
        for (int i = 0; i < obstaclesPref.Count; i++)
        {
            for (int j = 0; j < obstacleQuantity; j++)
            {
                GameObject go = Instantiate(obstaclesPref[i], transform.position, transform.rotation);
                go.transform.position = new Vector3(0, 0, 0);
            }
        }
    }
}
