using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemSpawnerTypes;

[CreateAssetMenu(fileName = "ItemSpawnerSetting", menuName = "ScriptableObjects/ItemSpawnerSetting")]
public class ItemSpawnerSettings : ScriptableObject
{
    public ItemSpawnerType ItemSpawnerType;
    public int LaneMaxCount = 1;
    public int NoItemSpawnChance = 1;
    public int PowerUpCooldownTimer = 1;
}
