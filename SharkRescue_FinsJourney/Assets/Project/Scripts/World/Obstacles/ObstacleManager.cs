using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObstacleTypes;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacles Creation")]
    [SerializeField] public List<ObstacleListPrefabs> ObstaclePrefabs = new();
    [SerializeField] private GameObject obstacleLanePrefab;


    [Header("Obstacles List")]
    public GameObject obstacleContainer;
    private GameObject ObstacleLaneContainer;
    public List<GameObject> disabledObstacleLanes;
    [SerializeField] private List<GameObject> allObstacleLanes;
    public int allActiveObstaclesCounter = new();
    [SerializeField] private List<int> spawnRate = new();

    [Header("Obstacles Settings")]
    [SerializeField] private int obstacleMovementSpeed = 10;
    [SerializeField] private int maxObstacleLanesShownAtTime = 3;
    [SerializeField] private int obstacleRespawnDistance = 5;
    public int distanceAdjustment = 0; //After the first obstacle has been deactivated, the new obstacle spawns accordingly at location (one spot earlier)

    [Header("Obstacles GameManager Settings")]
    [Range(1, 10), Tooltip("How many obstacles should be spawned in the same grid (1-8 out of 9).")]
    [SerializeField] private int gameDifficulty = 1;

    [Header("Small Obstacles Spawn Positions")]
    public List<Vector3> obstacleSpawnPositions = new();
    public float spawnAdjustment = 0f; //OnTriggerExit is not precisly enoguh (0.51;0.22;1.04) -> Difference needs to be added

    public static ObstacleManager Instance { get; private set; }

    private void Awake()
    {
        obstacleContainer = new()
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
        CreateLanes();
        for (int i = 0; i < maxObstacleLanesShownAtTime; i++)
        {
            SpawnObstacles();
        }
        AdjustAllActiveObstacle();
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
                    go.transform.parent = obstacleContainer.transform;
                    
                    ObstaclePrefabs[i].disabledObstacleList.Add(go);
                }
            }
            //Debug.Log("ObstaclePrefabs[i].spawnRate " + ObstaclePrefabs[i].spawnRate);
            spawnRate.Add(ObstaclePrefabs[i].spawnRate);
            ObstaclePrefabs[i].disabledObstacleList.Shuffle(); //Shuffle the list with "Fisher-Yates Shuffle"
        }
    }
    private void CreateLanes()
    {
        ObstacleLaneContainer = new GameObject() { name = "ObstacleLaneContainer"};
        for (int i = 0; i <= maxObstacleLanesShownAtTime; i++) //-> One extra Lane that deactivated for the next Obstacle
        {
            GameObject ObstacleLane = Instantiate(obstacleLanePrefab); ;
            ObstacleLane.name = $"ObstacleLane_{i:00}";
            ObstacleLane.transform.parent = ObstacleLaneContainer.transform;
            ObstacleLane.SetActive(false);
            disabledObstacleLanes.Add(ObstacleLane);
            allObstacleLanes.Add(ObstacleLane);
        }
    }

    public void AdjustAllActiveObstacle() //Adjust all Active Obstacles MovementSpeed <---------------------------------------------Muss noch geändert werden
    {
        for (int i = 0; i < allObstacleLanes.Count; i++) //How many prefabs lists exist?
        {
            allObstacleLanes[i].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
        }
    }
    
    public void SpawnObstacles()
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

            if (randChance <= counter && spawnRate[i] > 0)
            {
                allActiveObstaclesCounter++;

                ObstacleTypes.ObstacleType tempObstacleType = ObstaclePrefabs[i].obstacleType;

                switch (tempObstacleType)
                {
                    case ObstacleType.Small: //at least 1 slot from lane (min: 1/8)
                        int randMinValue = (int)ObstaclePrefabs[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].x;
                        int randMaxValue = (int)ObstaclePrefabs[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].y;
                        //Debug.Log("randMinValue: " + randMinValue + "; randMaxValue " + randMaxValue);
                        int maxSmallObstacleCounter = Random.Range(randMinValue, randMaxValue + 1);
                        //Debug.Log("maxSmallObstacleCounter " + maxSmallObstacleCounter);
                        List<Vector3> tempObstacleSpawnPositions = new (obstacleSpawnPositions);
                        tempObstacleSpawnPositions.Shuffle();

                        for (int j = 0; j < maxSmallObstacleCounter; j++)
                        {
                            int randGo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                            int randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List
                            //Debug.Log($"randGo {randGo}; randPos {randPos}");

                            if (j == 0)
                            {
                                disabledObstacleLanes[0].transform.position = new Vector3(-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment), 0, 0);
                            }

                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.position = new Vector3(-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment, tempObstacleSpawnPositions[randPos].y, tempObstacleSpawnPositions[randPos].z);
                            ObstaclePrefabs[i].disabledObstacleList[randGo].SetActive(true);

                            disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstaclePrefabs[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                            disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                            disabledObstacleLanes[0].SetActive(true);


                            ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[randGo]);
                            ObstaclePrefabs[i].disabledObstacleList.Remove(ObstaclePrefabs[i].disabledObstacleList[randGo]);
                            tempObstacleSpawnPositions.RemoveAt(randPos);
                            
                        }

                        disabledObstacleLanes.RemoveAt(0);

                        break;
                    case ObstacleType.Medium1: //1 whole lane (1x3 slots)

                        break;
                    case ObstacleType.Medium2: //2 whole lanes (2x3 slots)

                        break;
                    case ObstacleType.Big: //at least 2 whole lane (min: 2x3 slots)

                        int randgo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count); //Pick one random deactivated Big Obstacle
                        ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[randgo]);
                        
                        disabledObstacleLanes[0].transform.position = new Vector3(-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment, 0, 0); //disabledObstacleLanes[0] = The first freely available lane is used 
                        ObstaclePrefabs[i].disabledObstacleList[randgo].transform.parent = disabledObstacleLanes[0].transform;
                        ObstaclePrefabs[i].disabledObstacleList[randgo].transform.localPosition = new Vector3(0,0,0);
                        
                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstaclePrefabs[i].disabledObstacleList[randgo].GetComponent<Obstacle>());
                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                        disabledObstacleLanes[0].SetActive(true);
                        disabledObstacleLanes.RemoveAt(0);

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
