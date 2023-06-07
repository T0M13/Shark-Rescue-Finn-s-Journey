using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class ItemSpawnerNew : MonoBehaviour
{
    [Header("Item Creation")]
    public List<ItemListPrefabs> ItemPrefabs;

    [Header("Item List"), Tooltip("Will be created on runtime.")]
    public GameObject itemContainer;
    [SerializeField] private List<int> spawnRate = new();

    [Header("Item Spawn Settings")]
    [SerializeField] private List<Vector3> spawnPositions = new();
    [SerializeField] private Vector2 spawnPositionBuffer = new(6f, 15f);
    [SerializeField] private float spawnDistance = 5f;

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

        //CreateItems();
        //StartCoroutine(Spawn(65));
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
            spawnRate.Add(ItemPrefabs[i].spawnRate);
            ItemPrefabs[i].disabledItemList.Shuffle(); //Shuffle the list with "Fisher-Yates Shuffle"
        }
    }


    private void SpawnItemsOLD(float startSpawnDistance, int currentLaneCount, int currentItemCount)
    {
        int maxRange = 0;
        int currentSpawnRate = 0;
        List<Vector3> tempItemSpawnPos = new();
        int randItemsCount = 0;
        float randSpawnPosBuffer;

        for (int i = 0; i < spawnRate.Count; i++) //MaxRange for Random, gets from List "ItemPrefabs" -> SpawnRate
        {
            maxRange += spawnRate[i];
        }

        if (maxRange == 0)
            return;



        int randChance = Random.Range(1, maxRange + 1);

        for (int i = 0; i < spawnRate.Count; i++) //(randChance) Will be added until the random value is less than the current added spawnRate value (counter) -> With that check we know which Obstacle Typ got the chance to spawn
        {
            currentSpawnRate += spawnRate[i];

            if (randChance <= currentSpawnRate && spawnRate[i] > 0)
            {
                if(currentLaneCount == 0 && currentItemCount == 0)
                {
                    tempItemSpawnPos = new(spawnPositions);

                    randItemsCount = Random.Range(itemMinQuantity, itemMaxQuantity + 1);
                    randSpawnPosBuffer = Random.Range(spawnPositionBuffer.x, spawnPositionBuffer.y);
                }
                else if(currentLaneCount > 0 && currentItemCount == 0)
                {
                    randItemsCount = Random.Range(itemMinQuantity, itemMaxQuantity + 1);
                    randSpawnPosBuffer = Random.Range(spawnPositionBuffer.x, spawnPositionBuffer.y);
                }

                int randPos;
                int randGo;

                for (int j = currentLaneCount; j < itemLaneMaxCount; j++)
                {

                    randPos = Random.Range(0, tempItemSpawnPos.Count);
                    randGo = Random.Range(0, ItemPrefabs[i].disabledItemList.Count);


                    for (int k = 0; k < randItemsCount; k++)
                    {

                    }


                    tempItemSpawnPos.RemoveAt(randPos);
                }

            }
        }
    }

    public void SpawnItems(float startSpawnDistance)
    {
        List<Vector3> tempItemSpawnPos = new(spawnPositions);
        int randPos;
        int randItemsCount;
        float randSpawnPosBuffer;
        int maxRange;
        int currentSpawnRate = 0;
        int randChance;
        //Debug.Log("SpawnItems: " + startSpawnDistance);

        for (int i = 0; i < itemLaneMaxCount; i++) //How many random lanes can get items per chunk
        {
            //Debug.Log("itemLaneMaxCount: " + itemLaneMaxCount);
            randPos = Random.Range(0, tempItemSpawnPos.Count);
            randItemsCount = Random.Range(itemMinQuantity, itemMaxQuantity + 1);
            randSpawnPosBuffer = Random.Range(spawnPositionBuffer.x, spawnPositionBuffer.y);
            //Debug.Log("tempItemSpawnPos[randPos] " + tempItemSpawnPos[randPos]);
            Debug.Log("-------------randItemsCount " + randItemsCount);

            for (int j = 0; j < randItemsCount; j++) //How many random items can be spawned per lane
            {
                maxRange = 0;

                for (int k = 0; k < spawnRate.Count; k++) //MaxRange for Random, gets from List "ItemPrefabs" -> SpawnRate
                {
                    maxRange += spawnRate[k];
                }

                randChance = Random.Range(1, maxRange + 1);
                currentSpawnRate = 0;

                Debug.Log($"randChance {startSpawnDistance} {j}: {randChance}");

                for (int l = 0; l < spawnRate.Count; l++) //(randChance) Will be added until the random value is less than the current added spawnRate value (counter) -> With that check we know which Obstacle Typ got the chance to spawn
                {
                    currentSpawnRate += spawnRate[l];

                    if (randChance <= currentSpawnRate && spawnRate[l] > 0)
                    {
                        int randGo = Random.Range(0, ItemPrefabs[l].disabledItemList.Count);

                        ItemPrefabs[l].disabledItemList[randGo].transform.position = tempItemSpawnPos[randPos] + new Vector3(0, 0, startSpawnDistance + randSpawnPosBuffer + spawnDistance * j);
                        ItemPrefabs[l].disabledItemList[randGo].SetActive(true);
                        ItemPrefabs[l].activeItemList.Add(ItemPrefabs[l].disabledItemList[randGo]);
                        //Debug.Log("ItemPrefabs[l].disabledItemList[randGo]" + ItemPrefabs[l].disabledItemList[randGo]);
                        ItemPrefabs[l].disabledItemList[randGo].GetComponent<BaseItem>().MoveSpeed = itemMovementSpeed;

                        ItemPrefabs[l].disabledItemList.RemoveAt(randGo);

                        if (ItemPrefabs[l].itemTypes == ItemType.ItemTypes.PowerUp)
                        {
                            //Debug.Log("PowerUp: +" + ItemPrefabs[l].itemTypes);
                        }
                        break;
                    }
                }

            }


            tempItemSpawnPos.RemoveAt(randPos);
        }


    }


    public IEnumerator Spawn(float startSpawnDistance)
    {
        List<Vector3> tempItemSpawnPos = new(spawnPositions);
        int randPos;
        int randItemsCount;
        float randSpawnPosBuffer;
        int maxRange = 0;
        int currentSpawnRate = 0;
        int randChance;
        Debug.Log("SpawnItems: " + startSpawnDistance);

        for (int i = 0; i < itemLaneMaxCount; i++) //How many random lanes can get items per chunk
        {
            //Debug.Log("itemLaneMaxCount: " + itemLaneMaxCount);
            randPos = Random.Range(0, tempItemSpawnPos.Count);
            randItemsCount = Random.Range(itemMinQuantity, itemMaxQuantity + 1);
            randSpawnPosBuffer = Random.Range(spawnPositionBuffer.x, spawnPositionBuffer.y);
            //Debug.Log("tempItemSpawnPos[randPos] " + tempItemSpawnPos[randPos]);
            Debug.Log("-------------randItemsCount " + randItemsCount);

            for (int j = 0; j < randItemsCount; j++) //How many random items can be spawned per lane
            {
                maxRange = 0;
                for (int k = 0; k < spawnRate.Count; k++) //MaxRange for Random, gets from List "ItemPrefabs" -> SpawnRate
                {
                    maxRange += spawnRate[k];
                }

                Debug.Log("____________________maxRange " + maxRange);

                randChance = Random.Range(1, maxRange + 1);

                Debug.Log($"randChance {startSpawnDistance}: {j}");

                for (int l = 0; l < spawnRate.Count; l++) //(randChance) Will be added until the random value is less than the current added spawnRate value (counter) -> With that check we know which Obstacle Typ got the chance to spawn
                {
                    currentSpawnRate += spawnRate[l];

                    if (randChance <= currentSpawnRate && spawnRate[l] > 0)
                    {
                        int randGo = Random.Range(0, ItemPrefabs[l].disabledItemList.Count);

                        ItemPrefabs[l].disabledItemList[randGo].transform.position = tempItemSpawnPos[randPos] + new Vector3(0, 0, startSpawnDistance + randSpawnPosBuffer + spawnDistance * j);
                        ItemPrefabs[l].disabledItemList[randGo].SetActive(true);
                        ItemPrefabs[l].activeItemList.Add(ItemPrefabs[l].disabledItemList[randGo]);
                        //Debug.Log("ItemPrefabs[l].disabledItemList[randGo]" + ItemPrefabs[l].disabledItemList[randGo]);
                        ItemPrefabs[l].disabledItemList[randGo].GetComponent<BaseItem>().MoveSpeed = itemMovementSpeed;

                        ItemPrefabs[l].disabledItemList.RemoveAt(randGo);

                        if (ItemPrefabs[l].itemTypes == ItemType.ItemTypes.PowerUp)
                        {
                            //Debug.Log("PowerUp: +" + ItemPrefabs[l].itemTypes);
                        }
                        break;
                    }
                    yield return new WaitForSeconds(3);
                }

            }


            tempItemSpawnPos.RemoveAt(randPos);
        }


    }

}
