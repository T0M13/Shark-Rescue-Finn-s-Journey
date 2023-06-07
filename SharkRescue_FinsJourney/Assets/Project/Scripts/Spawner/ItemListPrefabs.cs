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
    [Tooltip("Here the respective prefabs are copied x times.")]
    public List<GameObject> disabledItemList = new();
    [Tooltip("Currently active Obstacles")]
    public List<GameObject> activeItemList = new();
    [Range(0, 100)]
    public int spawnRate = 25;
    public int itemQuantity = 20;
}
