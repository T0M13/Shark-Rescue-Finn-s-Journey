using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnvironmentType;
using static ObstacleTypes;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacles Creation")]
    [SerializeField] public List<ObstacleList> ObstacleList = new();
    [SerializeField] private GameObject obstacleLanePrefab;


    [Header("Obstacles List")]
    public GameObject obstacleContainer;
    private GameObject ObstacleLaneContainer;
    public List<GameObject> disabledObstacleLanes;
    [SerializeField] private List<GameObject> allObstacleLanes;
    public int allActiveObstaclesCounter = new(); //How many obstacles are currently active (Necessary for spawn distance ObstacleLanes)
    //[SerializeField] private List<int> spawnRate = new();
    //[SerializeField] private List<int> spawnHights = new();
    //[SerializeField] private List<Vector3> remainingSpawnSpots = new();

    [Header("Obstacles Settings")]
    [SerializeField] private EEnvironmentType eCurrentEnvironmentType;
    [SerializeField] private float obstacleMovementSpeed = 0f;
    [SerializeField] private int maxObstacleLanesShownAtTime = 5;
    [SerializeField] private int obstacleRespawnDistance = 5; //Obstacle Respawn Distance Multiplicator 13 * 5 -> 65 Distance between Obstacles
    public int distanceAdjustment = 0; //After the first obstacle has been deactivated, the new obstacle spawns accordingly at location (one spot earlier) (For X-Axis)
    public float spawnAdjustment = 0f; //OnTriggerExit is not precisly enoguh (0.51;0.22;1.04) -> Difference needs to be added

    [Header("Obstacles GameManager Settings")]
    [Range(1, 10), Tooltip("How many obstacles should be spawned in the same grid (1-8 out of 9).")]
    [SerializeField] private int gameDifficulty = 1;

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

        obstacleContainer = new() { name = "Obstacle Container" };
    }

    void Start()
    {

        CreateObstacles();
        CreateLanes();

        for (int i = 0; i < maxObstacleLanesShownAtTime; i++)
        {
            SpawnObstacles();
        }
        //AdjustAllActiveObstacle(); //obstacleMovementSpeed = GameManager.Instance
        AdjustMovementSpeed();
    }

    public void UpdateMovementSpeed(int newMovementSpeed)
    {

    }

    private void CreateObstacles()
    {

        for (int i = 0; i < ObstacleList.Count; i++) //How many prefabs lists exist?
        {
            for (int j = 0; j < ObstacleList[i].obstacleConfigurations.Count; j++)
            {
                for (int k = 0; k < ObstacleList[i].obstacleConfigurations[j].obstaclePrefabs.Count; k++) //How many prefabs exist in this list?
                {
                    for (int l = 0; l < ObstacleList[i].obstacleConfigurations[j].obstacleQuantity; l++) //How often should each prefab be instantiated?
                    {
                        GameObject go = Instantiate(ObstacleList[i].obstacleConfigurations[j].obstaclePrefabs[k], transform.position, transform.rotation);
                        go.SetActive(false);
                        go.transform.parent = obstacleContainer.transform;

                        ObstacleList[i].obstacleConfigurations[j].disabledObstacleList.Add(go);
                    }
                }
                ObstacleList[i].spawnRate.Add(ObstacleList[i].obstacleConfigurations[j].spawnRate);
                ObstacleList[i].obstacleConfigurations[j].disabledObstacleList.Shuffle(); //Shuffle the list with "Fisher-Yates Shuffle"
            }

        }
    }
    private void CreateLanes()
    {
        ObstacleLaneContainer = new GameObject() { name = "ObstacleLaneContainer" };
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

    public void AdjustMovementSpeed()
    {
        if (GameManager.Instance == null)
            return;


        obstacleMovementSpeed = GameManager.Instance.GameSpeed;
        //Debug.Log("obstacleMovementSpeed " + obstacleMovementSpeed);

        for (int i = 0; i < allObstacleLanes.Count; i++) //How many prefabs lists exist
        {
            allObstacleLanes[i].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
        }
    }

    public void SpawnObstacles()
    {
        int maxRange = 0;
        int currentSpawnRate = 0; //What is currently the value
        eCurrentEnvironmentType = GameManager.Instance.EEnvironmentTyp;

        for (int g = 0; g < ObstacleList.Count; g++)
        {
            if (ObstacleList[g].environmentType == eCurrentEnvironmentType)
            {

                for (int i = 0; i < ObstacleList[g].spawnRate.Count; i++) //MaxRange for Random, gets from List "ObstaclePrefabs" -> SpawnRate
                {
                    maxRange += ObstacleList[g].spawnRate[i];
                }
                //Debug.Log("maxRange " + maxRange);

                int randChance = Random.Range(1, maxRange + 1);
                //Debug.Log("randChance " + randChance);

                for (int h = 0; h < ObstacleList[g].obstacleConfigurations.Count; h++)
                {

                    for (int i = 0; i < ObstacleList[g].spawnRate.Count; i++) //(randChance) Will be added until the random value is less than the current added spawnRate value (counter) -> With that check we know which Obstacle Typ got the chance to spawn
                    {
                        currentSpawnRate += ObstacleList[g].spawnRate[i];

                        if (randChance <= currentSpawnRate && ObstacleList[g].spawnRate[i] > 0)
                        {
                            allActiveObstaclesCounter++;

                            ObstacleTypes.ObstacleSizeType tempObstacleType = ObstacleList[g].obstacleConfigurations[i].obstacleType; //Which random obstacle type has been chosen
                            List<Vector3> tempObstacleSpawnPositions = new(ObstacleList[g].obstacleConfigurations[i].obstacleSpawnPositions.SpawnPositions); //Temo list of possible spawn locations
                            
                            gameDifficulty = GameManager.Instance.GameDifficutly;                                                  //List<Vector3> remainingSpawnSpots = new();
                                                                                                                                   //List<Vector3> convertingRemainingSpawnSpots = new();
                            int randMinValue = (int)ObstacleList[g].obstacleConfigurations[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].x; //Random min value
                            int randMaxValue = (int)ObstacleList[g].obstacleConfigurations[i].additionalObstacleProbability.SpawnQuantityRange[gameDifficulty - 1].y; //Random max value
                            int randGo; //Random gameobject/obstacle
                            int randPos; //Random position
                            int maxSmallObstacleCounter = Random.Range(randMinValue, randMaxValue + 1); //Random count of how many obstacle should be created Random.Range(randMinValue, randMaxValue + 1)
                            float overallZDistancePos;


                            switch (tempObstacleType)
                            {
                                case ObstacleSizeType.Small: //at least 1 slot from lane (min: 1/8)

                                    tempObstacleSpawnPositions.Shuffle();

                                    overallZDistancePos = 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment;
                                    disabledObstacleLanes[0].transform.position = new Vector3(0, 0, overallZDistancePos);

                                    for (int j = 0; j < maxSmallObstacleCounter; j++)
                                    {
                                        randGo = Random.Range(0, ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                                        randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List
                                        int randRo = Random.Range(-90, 90); //One random Rotation for GO
                                        //Debug.Log("randRo " + randRo);

                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.position = new Vector3(tempObstacleSpawnPositions[randPos].x, tempObstacleSpawnPositions[randPos].y, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].gameObject.transform.rotation = Quaternion.Euler(0f,0f, randRo);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].SetActive(true);

                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().eCurrentEnvironmentType = eCurrentEnvironmentType;
                                        //;
                                        disabledObstacleLanes[0].SetActive(true);

                                        ObstacleList[g].obstacleConfigurations[i].activeObstacleList.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Remove(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                        tempObstacleSpawnPositions.RemoveAt(randPos);
                                    }

                                    //remainingSpawnSpots = new (tempObstacleSpawnPositions);
                                    disabledObstacleLanes.RemoveAt(0);

                                    ItemSpawnerNew.Instance.SpawnItems(overallZDistancePos);

                                    break;

                                case ObstacleSizeType.Medium1: //1 whole lane (1x3 slots)
                                    tempObstacleSpawnPositions.Shuffle();

                                    overallZDistancePos = 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment;
                                    disabledObstacleLanes[0].transform.position = new Vector3(0, 0, overallZDistancePos);

                                    for (int j = 0; j < maxSmallObstacleCounter; j++)
                                    {
                                        randGo = Random.Range(0, ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                                        randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List

                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.position = new Vector3(tempObstacleSpawnPositions[randPos].x, tempObstacleSpawnPositions[randPos].y, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].SetActive(true);

                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().eCurrentEnvironmentType = eCurrentEnvironmentType;
                                        disabledObstacleLanes[0].SetActive(true);

                                        ObstacleList[g].obstacleConfigurations[i].activeObstacleList.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Remove(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                        tempObstacleSpawnPositions.RemoveAt(randPos);
                                    }

                                    //convertingRemainingSpawnSpots = new(tempObstacleSpawnPositions);
                                    //for (int j = 0; j < convertingRemainingSpawnSpots.Count; j++)
                                    //{
                                    //    for (int k = 0; k < 3; k++) //1 whole lane (1x3 slots) -> convertingRemainingSpawnSpots.Count * 3 slots remain
                                    //    {
                                    //        remainingSpawnSpots.Add(new Vector3(convertingRemainingSpawnSpots[j].x, ObstaclePrefabs[0].obstacleSpawnPositions.SpawnPositions[k*3].y, 0));
                                    //
                                    //    }
                                    //}
                                    //Debug.Log($"remainingSpawnSpots Medium1 Count {remainingSpawnSpots.Count}");

                                    disabledObstacleLanes.RemoveAt(0);

                                    ItemSpawnerNew.Instance.SpawnItems(overallZDistancePos);

                                    break;
                                case ObstacleSizeType.Medium2: //2 whole lanes (2x3 slots)

                                    tempObstacleSpawnPositions.Shuffle();

                                    overallZDistancePos = 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment;
                                    disabledObstacleLanes[0].transform.position = new Vector3(0, 0, overallZDistancePos);

                                    for (int j = 0; j < maxSmallObstacleCounter; j++)
                                    {
                                        randGo = Random.Range(0, ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Count); //Pick one random deactivated Small Obstacle
                                        randPos = Random.Range(0, tempObstacleSpawnPositions.Count); //Pick one random Position from List

                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.position = new Vector3(tempObstacleSpawnPositions[randPos].x, tempObstacleSpawnPositions[randPos].y, 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].SetActive(true);

                                        //-13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment -3
                                        //65 zw. Objects
                                        //GameManager.instance.OnSpawnObject()

                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                                        disabledObstacleLanes[0].GetComponent<ObstacleLane>().eCurrentEnvironmentType = eCurrentEnvironmentType;
                                        disabledObstacleLanes[0].SetActive(true);

                                        ObstacleList[g].obstacleConfigurations[i].activeObstacleList.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                        ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Remove(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                        tempObstacleSpawnPositions.RemoveAt(randPos);
                                    }

                                    //convertingRemainingSpawnSpots = new(tempObstacleSpawnPositions);
                                    //Debug.Log("Medium2 convertingRemainingSpawnSpots[0].x * 2 " + convertingRemainingSpawnSpots[0].x * 2);
                                    //for (int k = 0; k < 3; k++) //1 whole lane (1x3 slots) -> convertingRemainingSpawnSpots.Count * 3 slots remain
                                    //{
                                    //    remainingSpawnSpots.Add(new Vector3(convertingRemainingSpawnSpots[0].x * 2, ObstaclePrefabs[0].obstacleSpawnPositions.SpawnPositions[k * 3].y, 0));
                                    //}
                                    //
                                    //Debug.Log($"remainingSpawnSpots Medium2 Count {remainingSpawnSpots.Count}");


                                    disabledObstacleLanes.RemoveAt(0);

                                    ItemSpawnerNew.Instance.SpawnItems(overallZDistancePos);

                                    break;
                                case ObstacleSizeType.Big: //at least 2 whole lane (min: 2x3 slots)

                                    randGo = Random.Range(0, ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.Count); //Pick one random deactivated Big Obstacle

                                    overallZDistancePos = 13 * obstacleRespawnDistance * (allActiveObstaclesCounter - distanceAdjustment) + spawnAdjustment;
                                    disabledObstacleLanes[0].transform.position = new Vector3(0, 0, overallZDistancePos);

                                    ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.parent = disabledObstacleLanes[0].transform;
                                    ObstacleList[g].obstacleConfigurations[i].activeObstacleList.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo]);
                                    ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].transform.localPosition = new Vector3(0, 0, 0);


                                    disabledObstacleLanes[0].GetComponent<ObstacleLane>().obstacles.Add(ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].GetComponent<Obstacle>());
                                    disabledObstacleLanes[0].GetComponent<ObstacleLane>().movementSpeed = obstacleMovementSpeed;
                                    disabledObstacleLanes[0].GetComponent<ObstacleLane>().eCurrentEnvironmentType = eCurrentEnvironmentType;
                                    disabledObstacleLanes[0].SetActive(true);
                                    disabledObstacleLanes.RemoveAt(0);

                                    ObstacleList[g].obstacleConfigurations[i].disabledObstacleList[randGo].SetActive(true);
                                    ObstacleList[g].obstacleConfigurations[i].disabledObstacleList.RemoveAt(randGo);

                                    ItemSpawnerNew.Instance.SpawnItems(overallZDistancePos);

                                    break;
                                default:
                                    break;
                            }

                            return;
                        }

                    }

                }
            }
        }

    }
}
