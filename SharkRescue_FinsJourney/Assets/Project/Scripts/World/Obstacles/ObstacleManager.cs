using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObstacleTypes;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacles Creation")]
    [SerializeField] public List<ObstacleListPrefabs> ObstaclePrefabs = new();

    [Header("Obstacles List")]
    //public List<GameObject> allDisabledObstacles = new();
    //public List<GameObject> allActiveObstacles = new();
    private GameObject obstacleParent;
    public int allActiveObstaclesCounter = new();
    [SerializeField] private List<int> spawnRate = new();

    [Header("Obstacles Settings")]
    [SerializeField] private int obstacleMovementSpeed = 10;
    [SerializeField] private int maxObstacleShownAtTime = 3;
    [SerializeField] private int obstacleRespawnDistance = 5;
    //[Range(1, 100), Tooltip("The probability of a small obstacle spawning instead of a big one.")]
    //[SerializeField] private int smallObstacleAppearRate = 80;
    public int distanceAdjustment = 0; //After the first obstacle has been deactivated, the new obstacle spawns accordingly at location (one spot earlier)

    [Header("Obstacles GameManager Settings")]
    [Range(1, 10), Tooltip("How many obstacles should be spawned in the same grid (1-8 out of 9).")]
    [SerializeField] private int gameDifficulty = 1;

    [Header("Small Obstacles Spawn Positions")]
    public List<Vector3> obstacleSpawnPositions = new();

    public static ObstacleManager Instance { get; private set; }

    private void Awake()
    {
        obstacleParent = new()
        {
            name = "Obstacle Container",
            //isStatic = true,
        };

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
        for (int i = 0; i < maxObstacleShownAtTime; i++)
        {
            SpawnObstacles();
        }

    }

    private void CreateObstacles()
    {
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

    public void AdjustAllActiveObstacle() //Adjust all Active Obstacles MovementSpeed
    {
        for (int i = 0; i < ObstaclePrefabs.Count; i++) //How many prefabs lists exist?
        {
            for (int j = 0; j < ObstaclePrefabs[i].activeObstacleList.Count; j++) //How many prefabs exist in this list?
            {
                ObstaclePrefabs[i].activeObstacleList[j].GetComponent<Obstacle>().movementSpeed = obstacleMovementSpeed;
            }
        }
    }
    
    public void SpawnObstacles()
    {
        int maxRange = 0;
        int counter = 0; //What is currently the value

        GameObject obstacleLane = new();
        obstacleLane.name = $"Obstacle Lane_{Random.Range(0,1000):0000}"; //:0000 == ToString("0000")
        obstacleLane.transform.parent = obstacleParent.transform;

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

            if (randChance < counter && spawnRate[i] > 0)
            {
                //Debug.Log("List i " + i);
                allActiveObstaclesCounter++;
                //Debug.Log("allActiveObstaclesCounter " + allActiveObstaclesCounter);

                ObstacleTypes.ObstacleType tempObstacleType = ObstaclePrefabs[i].obstacleType;

                switch (tempObstacleType)
                {
                    case ObstacleType.Small: //at least 1 slot from lane (min: 1/8)
                        int randMinValue = (int)ObstaclePrefabs[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].x;
                        int randMaxValue = (int)ObstaclePrefabs[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].y;
                        int maxSmallObstacleCounter = Random.Range(randMinValue, randMaxValue + 1);
                        List<Vector3> tempObstacleSpawnPositions = obstacleSpawnPositions;
                        tempObstacleSpawnPositions.Shuffle();

                        for (int j = 0; j < maxSmallObstacleCounter; j++)
                        {
                            int rand1 = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count);
                            int randPos = Random.Range(0, tempObstacleSpawnPositions.Count);

                            if(j == 0)
                            {
                                ObstaclePrefabs[i].disabledObstacleList[rand1].GetComponent<Obstacle>().canSpawnObstacle = true;
                            }
                            ObstaclePrefabs[i].disabledObstacleList[rand1].transform.position = new Vector3(-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment), tempObstacleSpawnPositions[randPos].y, tempObstacleSpawnPositions[randPos].z);
                            ObstaclePrefabs[i].disabledObstacleList[rand1].SetActive(true);
                            ObstaclePrefabs[i].disabledObstacleList[rand1].GetComponent<Obstacle>().movementSpeed = obstacleMovementSpeed;
                            ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[rand1]);
                            ObstaclePrefabs[i].disabledObstacleList.Remove(ObstaclePrefabs[i].disabledObstacleList[rand1]);
                            tempObstacleSpawnPositions.RemoveAt(randPos);
                            
                        }

                        break;
                    case ObstacleType.Medium1: //1 whole lane (1x3 slots)

                        break;
                    case ObstacleType.Medium2: //2 whole lanes (2x3 slots)

                        break;
                    case ObstacleType.Big: //at least 2 whole lane (min: 2x3 slots)

                        int randgo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count);
                        ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[randgo]);
                        ObstaclePrefabs[i].disabledObstacleList[randgo].GetComponent<Obstacle>().canSpawnObstacle = true;
                        ObstaclePrefabs[i].disabledObstacleList[randgo].transform.position = new Vector3(-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment), 0, 0);
                        ObstaclePrefabs[i].disabledObstacleList[randgo].GetComponent<Obstacle>().movementSpeed = obstacleMovementSpeed;
                        ObstaclePrefabs[i].disabledObstacleList[randgo].SetActive(true);
                        ObstaclePrefabs[i].disabledObstacleList.RemoveAt(randgo);

                        break;
                    default:
                        break;
                }



                return;
            }

        }

    }
}
