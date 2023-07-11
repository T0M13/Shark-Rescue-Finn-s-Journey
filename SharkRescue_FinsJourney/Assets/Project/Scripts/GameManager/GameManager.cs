using System;
using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEngine;
using static EnvironmentType;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;
    [SerializeField] private InGameUIManager inGameUIManager;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private ItemSpawnerNew itemSpawner;
    [SerializeField] private ObstacleManager obstacleManager;
    [SerializeField] private AudioManager audioManager;
    [Header("Game Environment")]
    [SerializeField] private EEnvironmentType eEnvironmentTyp = new();
    [Header("Game Positions")]
    [SerializeField] private Vector3 startPosition;
    [Header("Game Speed")]
    [SerializeField] private float originalSpeed = 15;
    [SerializeField] private float gameSpeed = 15;
    [SerializeField] private float gameSpeedMultiplier = 1.0f; // Initial speed multiplier
    [SerializeField] private float speedIncreaseRate = 0.1f; // Rate at which the speed multiplier increases over time
    [SerializeField] private float adjustedSpeed;
    [SerializeField] private float timer;
    [SerializeField] private float timerCooldown = 5f;
    [Header("Game Settings")]
    [SerializeField] private bool paused = false;
    [Header("Player Stats")]
    [SerializeField] private int health = 1;
    [SerializeField] private int coins;
    [SerializeField] private int score;
    [SerializeField] private float scoreTemp;
    private bool speedPowerup;
    [Header("Save/Load")]
    [SerializeField] private SaveComponent saveBehaviour;
    [SerializeField] private LoadComponent loadBehaviour;
    [Header("Saved Stats")]
    [SerializeField] private int playerProfileCoins;
    [SerializeField] private int playerProfileHighscore;
    [SerializeField] private int playerMaster;
    [SerializeField] private int playerMusic;
    [SerializeField] private int playerEffects;

    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
    public float GameSpeed { get => gameSpeed; set => gameSpeed = value; }
    public EEnvironmentType EEnvironmentTyp { get => eEnvironmentTyp; }
    public bool Paused { get => paused; set => paused = value; }

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

        if (chunkManager == null)
            chunkManager = FindObjectOfType<ChunkManager>();

        if (itemSpawner == null)
            itemSpawner = FindObjectOfType<ItemSpawnerNew>();

        if (obstacleManager == null)
            obstacleManager = FindObjectOfType<ObstacleManager>();

        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        adjustedSpeed = originalSpeed;

    }

    private void Start()
    {
        if (audioManager)
        {
            audioManager.Play("bubbles");
            audioManager.LoadVolumes();
        }
    }

    private void Update()
    {
        scoreTemp += Time.deltaTime;
        score = Mathf.RoundToInt(scoreTemp);

        if (inGameUIManager != null)
            inGameUIManager.CurrentScore.text = score.ToString();

        if (speedPowerup) return;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = timerCooldown;
            gameSpeed = adjustedSpeed;
            AdjustGameSpeed();
        }
        else
        {
            gameSpeedMultiplier += speedIncreaseRate * Time.deltaTime;
            adjustedSpeed = originalSpeed * gameSpeedMultiplier;
        }
    }

    private void AddCoin()
    {
        coins++;
        //Debug.Log("Current Coins: " + coins);
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

    private void OnValidate()
    {
        AdjustGameSpeed();
    }

    private void AdjustGameSpeed()
    {
        if (chunkManager != null)
            chunkManager.AdjustMovementSpeed();
        if (itemSpawner != null)
            itemSpawner.AdjustAllActiveItems();
        if (obstacleManager != null)
            obstacleManager.AdjustMovementSpeed();
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
        speedPowerup = true;
        gameSpeed = gameSpeed * 1.3f;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Paused = false;
    }


    public void ResetGameSpeed()
    {
        speedPowerup = false;
        gameSpeed = gameSpeed / 1.3f;
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
        playerMaster = SaveData.PlayerProfile.masterVolume;
        playerMusic = SaveData.PlayerProfile.musicVolume;
        playerEffects = SaveData.PlayerProfile.effectsVolume;



    }
}
