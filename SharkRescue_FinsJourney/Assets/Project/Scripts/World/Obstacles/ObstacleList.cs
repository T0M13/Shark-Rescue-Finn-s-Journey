using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnvironmentType;
using static ObstacleTypes;

[System.Serializable]
public class ObstacleList
{
    public EEnvironmentType environmentType = new();
    public List<ObstacleConfiguration> obstacleConfigurations = new();
    public List<int> spawnRate = new();

}

[System.Serializable]
public class ObstacleConfiguration
{
    public ObstacleSizeType obstacleType = new();
    [Tooltip("Unique Prefabs.")]
    public List<GameObject> obstaclePrefabs = new();
    [Tooltip("Here the respective prefabs are copied x times.")]
    public List<GameObject> disabledObstacleList = new();
    [Tooltip("Currently active Obstacles")]
    public List<GameObject> activeObstacleList = new();
    [Range(0, 100)]
    public int spawnRate = 25;
    public int obstacleQuantity = 3;
    public ObstacleProbability additionalObstacleProbability;
    public ObstacleSpawnPosition obstacleSpawnPositions;
}