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
    public int allActiveObstaclesCounter = new(); //How many obstacles are currently active (Necessary for spawn distance ObstacleLanes)
    [SerializeField] private List<int> spawnRate = new();

    [Header("Obstacles Settings")]
    [SerializeField] private int obstacleMovementSpeed = 10;
    [SerializeField] private int maxObstacleLanesShownAtTime = 3;
    [SerializeField] private int obstacleRespawnDistance = 5; //Obstacle Respawn Distance Multiplicator 13 * 5 -> 65 Distance between Obstacles
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
        AdjustAllActiveObstacle(); //obstacleMovementSpeed = GameManager.Instance
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

    private void AdjustAllActiveObstacle() 
    {
        for (int i = 0; i < allObstacleLanes.Count; i++) //How many prefabs lists exist
        {
            allObstacleLanes[i].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
        }
    }

    public void AdjustMovementSpeed() 
    {
        for (int i = 0; i < allObstacleLanes.Count; i++) //How many prefabs lists exist
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

                ObstacleTypes.ObstacleSizeType tempObstacleType = ObstaclePrefabs[i].obstacleType; //Which random obstacle type has been chosen
                List<Vector3> tempObstacleSpawnPositions = new(ObstaclePrefabs[i].obstacleSpawnPositions.SpawnPositions); //Temo list of possible spawn locations
                int randMinValue = (int)ObstaclePrefabs[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].x; //Random min value
                int randMaxValue = (int)ObstaclePrefabs[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].y; //Random max value
                int randGo; //Random gameobject/obstacle
                int randPos; //Random position
                int maxSmallObstacleCounter = Random.Range(randMinValue, randMaxValue + 1); //Random count of how many obstacle should be created Random.Range(randMinValue, randMaxValue + 1)

                switch (tempObstacleType)
                {
                    case ObstacleSizeType.Small: //at least 1 slot from lane (min: 1/8)
                        
                        tempObstacleSpawnPositions.Shuffle();

                        disabledObstacleLanes[0].transform.position = new Vector3(0, 0, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                        
                        for (int j = 0; j < maxSmallObstacleCounter; j++)
                        {
                            randGo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                            randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List

                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.position = new Vector3(tempObstacleSpawnPositions[randPos].x, tempObstacleSpawnPositions[randPos].y, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
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

                    case ObstacleSizeType.Medium1: //1 whole lane (1x3 slots)
                        tempObstacleSpawnPositions.Shuffle();

                        disabledObstacleLanes[0].transform.position = new Vector3(0, 0, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                        
                        for (int j = 0; j < maxSmallObstacleCounter; j++)
                        {
                            randGo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                            randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List

                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.position = new Vector3(tempObstacleSpawnPositions[randPos].x, tempObstacleSpawnPositions[randPos].y, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
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
                    case ObstacleSizeType.Medium2: //2 whole lanes (2x3 slots)

                        tempObstacleSpawnPositions.Shuffle();

                        disabledObstacleLanes[0].transform.position = new Vector3(0, 0, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                        
                        for (int j = 0; j < maxSmallObstacleCounter; j++)
                        {
                            randGo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                            randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List

                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                            ObstaclePrefabs[i].disabledObstacleList[randGo].transform.position = new Vector3(tempObstacleSpawnPositions[randPos].x, tempObstacleSpawnPositions[randPos].y, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                            ObstaclePrefabs[i].disabledObstacleList[randGo].SetActive(true);

                            //-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment -3
                            //65 zw. Objects
                            //GameManager.instance.OnSpawnObject()

                            disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstaclePrefabs[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                            disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                            disabledObstacleLanes[0].SetActive(true);

                            ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[randGo]);
                            ObstaclePrefabs[i].disabledObstacleList.Remove(ObstaclePrefabs[i].disabledObstacleList[randGo]);
                            tempObstacleSpawnPositions.RemoveAt(randPos);
                        }

                        disabledObstacleLanes.RemoveAt(0);

                        break;
                    case ObstacleSizeType.Big: //at least 2 whole lane (min: 2x3 slots)
                        
                        randGo = Random.Range(0, ObstaclePrefabs[i].disabledObstacleList.Count); //Pick one random deactivated Big Obstacle

                        ObstaclePrefabs[i].activeObstacleList.Add(ObstaclePrefabs[i].disabledObstacleList[randGo]);
                        
                        disabledObstacleLanes[0].transform.position = new Vector3(0, 0, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment); //disabledObstacleLanes[0] = The first freely available lane is used 
                        ObstaclePrefabs[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                        ObstaclePrefabs[i].disabledObstacleList[randGo].transform.localPosition = new Vector3(0,0,0);
                        
                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstaclePrefabs[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                        disabledObstacleLanes[0].SetActive(true);
                        disabledObstacleLanes.RemoveAt(0);

                        ObstaclePrefabs[i].disabledObstacleList[randGo].SetActive(true);
                        ObstaclePrefabs[i].disabledObstacleList.RemoveAt(randGo);

                        break;
                    default:
                        break;
                }

                return;
            }

        }

    }

    private void SpawnObstacleConfig()
    {
        
    }
}
