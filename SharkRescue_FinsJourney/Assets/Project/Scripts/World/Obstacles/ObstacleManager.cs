using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObstacleTypes;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacles Creation")]
    [SerializeField] private List<ObstacleListPrefabs> ObstaclePrefabs = new();

    [Header("Obstacles List")]
    public List<GameObject> allDisabledObstacles = new();
    public List<GameObject> allActiveObstacles = new();
    [SerializeField] private List<int> spawnRate = new();

    [Header("Obstacles Settings")]
    [SerializeField] private int obstacleMovingSpeed = 10;
    [SerializeField] private int maxObstacleShownAtTime = 3;
    [SerializeField] private int obstacleRespawnDistance = 5;
    //[Range(1, 100), Tooltip("The probability of a small obstacle spawning instead of a big one.")]
    //[SerializeField] private int smallObstacleAppearRate = 80;
    private int distancMultipl = 0;

    [Header("Obstacles Settings")]
    [Range(1, 8), Tooltip("How many obstacles should be spawned in the same grid (1-8 out of 9).")]
    [SerializeField] private int gameDifficulty = 1;

    [Header("Small Obstacles Spawn Positions")]
    public List<Vector3> obstacleSpawnPositions = new();


    //private int chunklength Awake = CreateChunks.Instance.chunklength;
    //private int chunkwidth Awake = CreateChunks.Instance.chunkwidth;

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

        CreateObstacles();
        //AdjustAllActiveObstacle();
        for (int i = 0; i < maxObstacleShownAtTime; i++)
        {
            SpawnObstacles();
        }

    }

    private void CreateObstacles()
    {
        GameObject obstacleParent = new()
        {
            name = "Obstacle Container"
        };

        for (int i = 0; i < ObstaclePrefabs.Count; i++) //How many prefabs lists exist?
        {
            for (int j = 0; j < ObstaclePrefabs[i].obstaclePrefabs.Count; j++) //How many prefabs exist in this list?
            {
                for (int k = 0; k < ObstaclePrefabs[i].obstacleQuantity; k++) //How often should each prefab be instantiated?
                {
                    GameObject go = Instantiate(ObstaclePrefabs[i].obstaclePrefabs[j], transform.position, transform.rotation);
                    go.SetActive(false);
                    go.transform.parent = obstacleParent.transform;

                    ObstaclePrefabs[i].disabledObstacleList.Add(go);
                }
            }
            Debug.Log("ObstaclePrefabs[i].spawnRate " + ObstaclePrefabs[i].spawnRate);
            spawnRate.Add(ObstaclePrefabs[i].spawnRate);
            ObstaclePrefabs[i].disabledObstacleList.Shuffle(); //Shuffle the list with "Fisher-Yates Shuffle"
        }
    }

    private void AdjustAllActiveObstacle()
    {
        //for (int i = 0; i < bigObstacle.Count; i++)
        //{
        //    bigObstacle[i].GetComponent<Obstacle>().movingSpeed = obstacleMovingSpeed;
        //}

        //for (int i = 0; i < ObstaclePrefabs.Count; i++) //How many prefabs lists exist?
        //{
        //    for (int j = 0; j < ObstaclePrefabs[i].obstaclePrefabs.Count; j++) //How many prefabs exist in this list?
        //    {
        //        GameObject go = Instantiate(ObstaclePrefabs[i].obstaclePrefabs[j], transform.position, transform.rotation);
        //        go.SetActive(false);
        //        disabledObstacles.Add(go);

        //    }
        //}



        ////if (i > maxObstacleShownAtTime - 1)
        ////{
        ////    bigObstacle[i].SetActive(false);
        ////    disabledObstacles.Add(bigObstacle[i]);
        ////    Debug.Log(bigObstacle[i] + " is SetActive false");
        ////}

        //for (int j = 0; j < maxObstacleShownAtTime; j++)
        //{
        //    bigObstacle[j].transform.position = new Vector3(-13 * obstacleRespawnDistance * (j + 1), 0, 0);
        //}
    }
    
    private void SpawnObstacles()
    {
        int maxRange = 0;
        int counter = 0; //What is currently the value
       
        for (int i = 0; i < spawnRate.Count; i++) //MaxRange for Random, gets from List "Obstacle Prefabs" -> SpawnRate
        {
            maxRange += spawnRate[i];
        }
        //Debug.Log("maxRange " + maxRange);

        int randChance = Random.Range(1, maxRange + 1);
        //Debug.Log("randChance " + randChance);

        for (int i = 0; i < spawnRate.Count; i++) //(randChance) Will be added until the random value is less than the current added spawnRate value (counter) -> With that check we know which Obstacle Typ got the chance to spawn
        {
            counter += spawnRate[i];

            if (randChance < counter && counter != 0)
            {
            //Debug.Log("List i " + i);
                int randgo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count);
                ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[randgo]);
                allActiveObstacles.Add(ObstaclePrefabs[i].disabledObstacleList[randgo]);

                AdjustSpawnDistance();
                ObstaclePrefabs[i].disabledObstacleList[randgo].SetActive(true);
                if(allActiveObstacles.Count <= maxObstacleShownAtTime)
                {
                    ObstaclePrefabs[i].disabledObstacleList[randgo].transform.position = new Vector3(-13 * obstacleRespawnDistance * allActiveObstacles.Count, 0, 0);
                    Debug.Log("allActiveObstacles.Count " + allActiveObstacles.Count);
                }
                else //Unnötig?
                {
                    ObstaclePrefabs[i].disabledObstacleList[randgo].transform.position = new Vector3(-13 * obstacleRespawnDistance * distancMultipl, 0, 0);
                    Debug.Log("distancMultipl " + distancMultipl);
                }
                ObstaclePrefabs[i].disabledObstacleList.RemoveAt(randgo);

                return;
            }

        }

    }

    private void AdjustSpawnDistance() //Unnötig?
    {
        //Checks if distancMultipl would be 0 (Cannot)
        if (maxObstacleShownAtTime - 2 <= 0)
        {
            distancMultipl = 1;
        }
        else
        {
            distancMultipl = maxObstacleShownAtTime - 1;
        }
    }

    public void AddNewObstacle()
    {
        //int rand = Random.Range(0, allDisabledObstacles.Count);

        ////Checks if distancMultipl would be 0 (Cannot)
        //if (maxObstacleShownAtTime - 2 <= 0)
        //{
        //    distancMultipl = 1;
        //}
        //else
        //{
        //    distancMultipl = maxObstacleShownAtTime - 1;
        //}

        //ObstacleTypes.ObstacleType _obstacleType = allDisabledObstacles[rand].GetComponent<Obstacle>().obstacleType;

        //switch (_obstacleType)
        //{
        //    case ObstacleTypes.ObstacleType.NONE:
        //        //Nothing
        //        break;
        //    case ObstacleTypes.ObstacleType.Small:
        //        AddSmallObstacle(rand);
        //        break;
        //    case ObstacleTypes.ObstacleType.Big:
        //        AddBigObstacle(rand);
        //        break;
        //    default:
        //        break;
        //}
        //allDisabledObstacles[rand].SetActive(true);
        //allDisabledObstacles.RemoveAt(rand);

    }

    private void AddBigObstacle(int rand)
    {
        allDisabledObstacles[rand].transform.position = new Vector3(-13 * obstacleRespawnDistance * distancMultipl, 0, 0);
    }
    private void AddSmallObstacle(int rand)
    {
        int counter = 0;
        List<Vector3> tempObstacleSpawnPositions = obstacleSpawnPositions;

        do
        {


            counter++;
        } while (counter > gameDifficulty);
    }
}
