using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleSpawnPos",menuName = "ScriptableObjects/SpawnPositions/Obstacle")]
public class ObstacleSpawnPosition : ScriptableObject
{
    public ObstacleTypes.ObstacleSizeType ObstacleSizeTypes;
    public Vector3[] SpawnPositions;
    public Vector3[] BlackListSpawnPositions;
}
