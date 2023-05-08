using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacles Creation")]
    [SerializeField] private List<GameObject> bigObstaclesPrefabs = new();
    [SerializeField] private List<GameObject> smallObstaclePrefabs = new();
    [Range(1, 20), Tooltip("How many of each obstacle should be generated.")]
    public int bigObstacleQuantity = 3;
    public int smallObstadcleQuantity = 3;

    [Header("Obstacles List")]
    [SerializeField] private List<GameObject> bigObstacle = new();
    [SerializeField] private List<GameObject> smallObstacle = new();
    public List<GameObject> disabledObstacles = new();

    [Header("Obstacles Settings")]
    [SerializeField] private int obstacleMovingSpeed = 10;
    [SerializeField] private int maxObstacleShownAtTime = 3;
    [SerializeField] private int obstacleRespawnDistance = 5;
    private int distancMultipl = 0;

    [Header("Obstacles Settings")]
    [Range(1, 8), Tooltip("How many obstacles should be spawned in the same grid (1-8 out of 9).")]
    [SerializeField] private int gameDifficulty = 1;
    

    [Header("Small Obstacles Spawn Positions")]
    public List<Vector3> obstacleSpawnPositions = new();


    private int chunklength = 0;
    private int chunkwidth = 0;

    public static ObstacleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        chunklength = CreateChunks.Instance.chunklength;
        chunkwidth = CreateChunks.Instance.chunkwidth;

        CreateObstacle();

        AdjustAllObstacle();
    }

    private void CreateObstacle()
    {
        for (int i = 0; i < bigObstaclesPrefabs.Count; i++)
        {
            for (int j = 0; j < bigObstacleQuantity; j++)
            {
                GameObject go = Instantiate(bigObstaclesPrefabs[i], transform.position, transform.rotation);
                //go.transform.position = new Vector3(-obstacleRespawnDistance, 0, 0);
                bigObstacle.Add(go);
            }
        }
    }

    private void AdjustAllObstacle()
    {
        for (int i = 0; i < bigObstacle.Count; i++)
        {
            bigObstacle[i].GetComponent<Obstacle>().movingSpeed = obstacleMovingSpeed;

            if (i > maxObstacleShownAtTime - 1)
            {
                bigObstacle[i].SetActive(false);
                disabledObstacles.Add(bigObstacle[i]);
                Debug.Log(bigObstacle[i] + " is SetActive false");
            }
        }

        for (int j = 0; j < maxObstacleShownAtTime; j++)
        {
            bigObstacle[j].transform.position = new Vector3(-13 * obstacleRespawnDistance * (j+1), 0, 0);
        }
    }

    public void AddNewObstacle()
    {
        int rand = Random.Range(0, disabledObstacles.Count);

        //Checks if distancMultipl would be 0 (Cannot)
        if (maxObstacleShownAtTime - 2 <= 0)
        {
            distancMultipl = 1;
        }
        else
        {
            distancMultipl = maxObstacleShownAtTime - 1;
        }

        ObstacleTypes.ObstacleType _obstacleType = disabledObstacles[rand].GetComponent<Obstacle>().obstacleType;

        switch (_obstacleType)
        {
            case ObstacleTypes.ObstacleType.NONE:
                //Nothing
                break;
            case ObstacleTypes.ObstacleType.Small:
                AddSmallObstacle(rand);
                break;
            case ObstacleTypes.ObstacleType.Big:
                AddBigObstacle(rand);
                break;
            default:
                break;
        }
        disabledObstacles[rand].SetActive(true);
        disabledObstacles.RemoveAt(rand);

    }

    private void AddBigObstacle(int temp)
    {
        disabledObstacles[temp].transform.position = new Vector3(-13 * obstacleRespawnDistance * distancMultipl, 0, 0);
    }
    private void AddSmallObstacle(int temp)
    {
        int counter = 0;
        List<Vector3> tempObstacleSpawnPositions = obstacleSpawnPositions;
        
        do
        {


            counter++;
        } while (counter > gameDifficulty);
    }


    //private static System.Random rng = new System.Random();

    //public static void Shuffle<T>(this IList<T> list)
    //{
    //    int n = list.Count;
    //    while (n > 1)
    //    {
    //        n--;
    //        int k = rng.Next(n + 1);
    //        T value = list[k];
    //        list[k] = list[n];
    //        list[n] = value;
    //    }
    //}
}
