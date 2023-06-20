using System;
using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;
    [SerializeField] private InGameUIManager inGameUIManager;
    [Header("Game Positions")]
    [SerializeField] private Vector3 startPosition;
    [Header("Game Speed")]
    [SerializeField] private int gameSpeed = 1;
    [Header("Player Stats")]
    [SerializeField] private int health = 1;
    [SerializeField] private int coins;
    [SerializeField] private int score;
    [SerializeField] private float scoreTemp;
    [Header("Save/Load")]
    [SerializeField] private SaveComponent saveBehaviour;
    [SerializeField] private LoadComponent loadBehaviour;
    [Header("Saved Stats")]
    [SerializeField] private int playerProfileCoins;
    [SerializeField] private int playerProfileHighscore;

    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
    public int GameSpeed { get => gameSpeed; set => gameSpeed = value; }

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

        if (playerReferences == null)
            playerReferences = FindObjectOfType<PlayerReferences>();

        if (inGameUIManager == null)
            inGameUIManager = FindObjectOfType<InGameUIManager>();

    }

    private void Update()
    {
        scoreTemp += Time.deltaTime;
        score = Mathf.RoundToInt(scoreTemp);

        if (inGameUIManager != null)
            inGameUIManager.CurrentScore.text = score.ToString();
    }

    private void AddCoin()
    {
        coins++;
        Debug.Log("Current Coins: " + coins);
    }

    private void GameOver()
    {
        SaveData.PlayerProfile.coins += coins;
        if (SaveData.PlayerProfile.highscore < score)
            SaveData.PlayerProfile.highscore = score;
        Save();
        Debug.Log("Game Over");
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
        item.MoveSpeed = GameSpeed;
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
        playerProfileHighscore = SaveData.PlayerProfile.highscore;
    }
}
