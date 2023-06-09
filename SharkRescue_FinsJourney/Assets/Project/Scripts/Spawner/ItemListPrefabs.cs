using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemType;
using static PowerUpTypes;

[System.Serializable]
public class ItemListPrefabs
{
    public ItemTypes itemTypes = new();
    public PowerUpType powerUpType = new();
    [Tooltip("Unique Prefabs.")]
    public List<GameObject> itemPrefabs = new();
    [Tooltip("Here the respective prefabs are stored.")]
    public List<GameObject> disabledItemList = new();
    [Range(0, 100), Tooltip("The Possibilty to Spawn")]
    public List<GameObject> activeItemList = new();
    [Tooltip("Currently active Obstacles")]
    public int itemQuantity = 20;
    public bool canSpawn = true;
    [Tooltip("How many Copies should be created.")]
    public float spawnRate = 25;
    [Tooltip("When PowerUp do not spawn, it will be higher next time.")]
    public float addSpawnChance = 0f;
}
