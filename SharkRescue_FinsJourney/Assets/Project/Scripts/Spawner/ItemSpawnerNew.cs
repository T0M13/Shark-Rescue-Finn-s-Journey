using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class ItemSpawnerNew : MonoBehaviour
{
    [Header("Item Creation")]
    public List<ItemListPrefabs> ItemPrefabs;

    [Header("Item List"), Tooltip("Will be created on runtime and list all Items in it.")]
    public GameObject itemContainer;
    [SerializeField] private List<float> defaultSpawnRate = new();
    [SerializeField] private List<float> currentSpawnRate = new();

    [Header("Item Spawn Settings")]
    [SerializeField] private List<Vector3> spawnPositions = new();
    [SerializeField] private Vector2 spawnPositionBuffer = new(6f, 15f);
    [SerializeField] private float spawnDistance = 5f;
    [Space(10)]

    [Tooltip("Chance that there will not spawn an Item.")]
    [SerializeField, Range(0, 100)] private int noItemSpawnChance = 10;
    [Tooltip("The same PowerUp will have a cooldown until it can spawn again.")]
    [SerializeField] private float powerUpCooldownTimer = 5;
    [Tooltip("When PowerUp do not spawn, it will be higher next time")]
    [SerializeField] private int powerUpAddChance = 1;
    [Space(10)]
    [SerializeField] private int itemMinQuantity = 5;
    [SerializeField] private int itemMaxQuantity = 9;
    [SerializeField] private int itemLaneMaxCount = 2;
    [SerializeField] private int itemMovementSpeed = 15;



    public static ItemSpawnerNew Instance { get; private set; }

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


        itemContainer = new() { name = "Item Container" };

        CreateItems();
    }

    void Start()
    {

    }

    private void CreateItems()
    {
        for (int i = 0; i < ItemPrefabs.Count; i++) //How many prefabs lists exist?
        {
            for (int j = 0; j < ItemPrefabs[i].itemPrefabs.Count; j++) //How many prefabs exist in this list?
            {
                for (int k = 0; k < ItemPrefabs[i].itemQuantity; k++) //How often should each prefab be instantiated?
                {
                    GameObject go = Instantiate(ItemPrefabs[i].itemPrefabs[j], transform.position, transform.rotation);
                    go.SetActive(false);
                    go.transform.parent = itemContainer.transform;

                    ItemPrefabs[i].disabledItemList.Add(go);
                }
            }
            defaultSpawnRate.Add(ItemPrefabs[i].spawnRate);
            ItemPrefabs[i].disabledItemList.Shuffle(); //Shuffle the list with "Fisher-Yates Shuffle"
        }

        defaultSpawnRate.Add(noItemSpawnChance);

        currentSpawnRate = new(defaultSpawnRate);
    }

    public void SpawnItems(float startSpawnDistance)
    {
        List<Vector3> tempItemSpawnPos = new(spawnPositions);
        int randPos;
        int randItemsCount;
        float randSpawnPosBuffer;
        float maxRange;
        float currentSpawnRateValue;
        float randChance;
        //Debug.Log("SpawnItems: " + startSpawnDistance);

        for (int i = 0; i < itemLaneMaxCount; i++) //How many random lanes can get items per chunk
        {
            tempItemSpawnPos.Shuffle();
            randPos = Random.Range(0, tempItemSpawnPos.Count);
            randItemsCount = Random.Range(itemMinQuantity, itemMaxQuantity + 1);
            randSpawnPosBuffer = Random.Range(spawnPositionBuffer.x, spawnPositionBuffer.y);
            //Debug.Log("itemLaneMaxCount: " + itemLaneMaxCount);
            //Debug.Log("tempItemSpawnPos[randPos] " + tempItemSpawnPos[randPos]);
            //Debug.Log("-------------randItemsCount " + randItemsCount);

            for (int j = 0; j < randItemsCount; j++) //How many random items can be spawned per lane
            {
                maxRange = 0;

                for (int k = 0; k < currentSpawnRate.Count; k++) //MaxRange for Random, gets from List "ItemPrefabs" -> SpawnRate
                {
                    maxRange += currentSpawnRate[k];
                }

                randChance = Random.Range(1, maxRange + 1);
                currentSpawnRateValue = 0;

                //Debug.Log($"randChance {startSpawnDistance} {j}: {randChance}");

                for (int l = 0; l < currentSpawnRate.Count; l++) //(randChance) Will be added until the random value is less than the current added spawnRate value (counter) -> With that check we know which Obstacle Typ got the chance to spawn
                {
                    currentSpawnRateValue += currentSpawnRate[l];

                    ////Mini summary
                    ////randChance <= currentSpawnRateValue && currentSpawnRate[l] > 0 --> Checking if "currentSpawnRateValue" (ItemPrefabs[l].spawnRate) >= randChance && currentSpawnRate[l] > 0
                    ////ItemPrefabs[l].canSpawn --> can spawn?
                    ////ItemPrefabs[l].disabledItemList.Count > 0 --> ItemPrefabs still available
                    if (randChance <= currentSpawnRateValue && currentSpawnRate[l] > 0 && l < currentSpawnRate.Count - 1 && ItemPrefabs[l].canSpawn && ItemPrefabs[l].disabledItemList.Count > 0)
                    {
                        int randGo = Random.Range(0, ItemPrefabs[l].disabledItemList.Count);

                        ItemPrefabs[l].disabledItemList[randGo].transform.position = tempItemSpawnPos[randPos] + new Vector3(0, 0, startSpawnDistance + randSpawnPosBuffer + spawnDistance * j);
                        ItemPrefabs[l].disabledItemList[randGo].SetActive(true);
                        
                        ItemPrefabs[l].activeItemList.Add(ItemPrefabs[l].disabledItemList[randGo]);

                        ItemPrefabs[l].disabledItemList[randGo].GetComponent<BaseItem>().MoveSpeed = itemMovementSpeed;
                        ItemPrefabs[l].disabledItemList.RemoveAt(randGo);

                        if (ItemPrefabs[l].itemTypes == ItemType.ItemTypes.PowerUp)
                        {
                            StartCoroutine(DeactivateSpawnChance(l));
                        }

                        for (int m = 0; m < ItemPrefabs.Count; m++)
                        {
                            if(ItemPrefabs[m].itemTypes == ItemType.ItemTypes.PowerUp && m != l && ItemPrefabs[m].canSpawn)
                            {
                                currentSpawnRate[m] += ItemPrefabs[m].addSpawnChance;
                            }
                        }
                        break;
                    }

                }

            }


            tempItemSpawnPos.RemoveAt(randPos);
        }


    }

    /// <summary>
    /// Set current PowerUp "canSpawn" false for the next "powerUpCooldownTimer" seconds
    /// </summary>
    /// <param name="currentPowerUp">Current PowerUp which should be deactivated (ItemPrefabs[?])</param>
    /// <returns></returns>
    public IEnumerator DeactivateSpawnChance(int currentPowerUp)
    {
        ItemPrefabs[currentPowerUp].canSpawn = false;
        currentSpawnRate[currentPowerUp] = defaultSpawnRate[currentPowerUp];

        yield return new WaitForSeconds(powerUpCooldownTimer);

        ItemPrefabs[currentPowerUp].canSpawn = true;
    }

    public void AdjustAllActiveItems()
    {
        for (int i = 0; i < ItemPrefabs.Count; i++)
        {
            for(int j = 0; j < ItemPrefabs[i].activeItemList.Count; j++)
            {
                ItemPrefabs[i].activeItemList[j].GetComponent<BaseItem>().MoveSpeed = itemMovementSpeed;
            }
        }
    }


}
