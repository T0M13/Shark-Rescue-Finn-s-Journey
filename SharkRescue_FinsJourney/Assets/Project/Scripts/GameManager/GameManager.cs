using System;
using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private PlayerReferences playerReferences;
    [Header("Game Positions")]
    [SerializeField] private Vector3 startPosition;
    [Header("Item Settings")]
    [SerializeField] private int itemsSpeed = 1;
    [Header("Player Stats")]
    [SerializeField] private int health = 1;
    [SerializeField] private int coins;
    [Header("Save/Load")]
    [SerializeField] private SaveComponent saveBehaviour;
    [SerializeField] private LoadComponent loadBehaviour;
    [Header("Saved Stats")]
    [SerializeField] private int playerProfileCoins;

    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }

    public Action OnAddCoin;
    public Action<BaseItem> OnSpawnObject;
    public Action<int> OnGetDamage;
    public Action<GameObject> OnDeactivateGObject;
    public Action OnSave;
    public Action OnLoad;
    public Action OnGameOver;

    public Action OnMagnetPowerUp;
    public Action OnSpeedPowerUp;

    private void OnEnable()
    {
        OnAddCoin += AddCoin;
        OnSave += Save;
        OnLoad += Load;
        OnGameOver += GameOver;
        OnDeactivateGObject += DeactivateGameObject;
        OnGetDamage += GetDamage;

        OnSpawnObject += SetObjectSpeed;
        OnMagnetPowerUp += MagnetPowerUp;
        OnSpeedPowerUp += SpeedPowerUp;
    }

    private void OnDisable()
    {
        OnAddCoin -= AddCoin;
        OnSave -= Save;
        OnLoad -= Load;
        OnGameOver -= GameOver;
        OnDeactivateGObject -= DeactivateGameObject;
        OnGetDamage -= GetDamage;

        OnSpawnObject -= SetObjectSpeed;
        OnMagnetPowerUp -= MagnetPowerUp;
        OnSpeedPowerUp -= SpeedPowerUp;


    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Load();
        Save();


        if (itemSpawner == null)
            itemSpawner = FindObjectOfType<ItemSpawner>();

        if (playerReferences == null)
            playerReferences = FindObjectOfType<PlayerReferences>();
    }

    private void AddCoin()
    {
        coins++;
        Debug.Log("Current Coins: " + coins);
    }

    private void GameOver()
    {
        SaveData.PlayerProfile.coins += coins;
        Save();
        Debug.Log("Game Over");
    }

    private void DeactivateGameObject(GameObject gObject)
    {
        if (itemSpawner == null)
            itemSpawner = FindObjectOfType<ItemSpawner>();
        if (itemSpawner == null) return;
        itemSpawner.DeactivateObject(gObject);
    }

    public void GetDamage(int damageValue)
    {
        health -= damageValue;
        if (health <= 0)
        {
            GameOver();
        }
    }

    private void SetObjectSpeed(BaseItem item)
    {
        item.MoveSpeed = itemsSpeed;
    }

    private void MagnetPowerUp()
    {
        if (playerReferences == null)
            playerReferences = FindObjectOfType<PlayerReferences>();
        if (playerReferences == null) return;

        playerReferences.PlayerInteractor.OnMagnetPowerUp?.Invoke();
    }

    private void SpeedPowerUp()
    {
        if (playerReferences == null)
            playerReferences = FindObjectOfType<PlayerReferences>();
        if (playerReferences == null) return;

        playerReferences.PlayerController.OnSpeedPowerUp?.Invoke();
    }

    private void Save()
    {
        saveBehaviour.Save();
    }

    private void Load()
    {
        loadBehaviour.Load();

        playerProfileCoins = SaveData.PlayerProfile.coins;
    }
}
