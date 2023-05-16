using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleProbability",menuName = "ScriptableObjects/Probability/Obstacle"),]
[System.Serializable]
public class ObstacleProbability : ScriptableObject
{
    public ObstacleTypes.ObstacleSizeType ObstacleSizeTypes;
    public Vector2[] SpawnQuantityRange;
}
