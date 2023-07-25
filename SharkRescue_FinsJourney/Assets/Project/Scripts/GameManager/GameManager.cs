using System;
using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private bool runTimer = true;
    [SerializeField] private const float timeSpeedCap = 55f;
    [Header("Game Settings")]
    [SerializeField] private bool paused = false;
    [SerializeField] private bool gameOver = false;
    [SerializeField] private int reAddHealthTime = 10;
    [Header("Player Stats")]
    [SerializeField] private int health = 1;
    [SerializeField] private int coins;
    [SerializeField] private int score;
    [SerializeField] private float scoreTemp;
    [SerializeField] private bool invincible;
    private bool starPowerUp;
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
    public bool GameOverEG { get => gameOver; set => gameOver = value; }

    public Action OnAddCoin;
    public Action<BaseItem> OnSpawnObject;
    public Action<int> OnGetDamage;
    public Action OnSave;
    public Action OnLoad;
    public Action OnGameOver;
    public Action OnReAddHealth;

    public Action OnMagnetPowerUp;
    public Action OnStarPowerUp;

    private void OnEnable()
    {
        OnAddCoin += AddCoin;
        OnSave += Save;
        OnLoad += Load;
        OnGameOver += GameOver;
        OnReAddHealth += ReAddHealth;
        OnGetDamage += GetDamage;

        OnMagnetPowerUp += MagnetPowerUp;
        OnStarPowerUp += StarPowerUp;
    }

    private void OnDisable()
    {
        OnAddCoin -= AddCoin;
        OnSave -= Save;
        OnLoad -= Load;
        OnGameOver -= GameOver;
        OnReAddHealth -= ReAddHealth;
        OnGetDamage -= GetDamage;

        OnMagnetPowerUp -= MagnetPowerUp;
        OnStarPowerUp -= StarPowerUp;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
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
        runTimer = true;
        gameOver = false;
        paused = false;
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
        if (gameOver) return;

        scoreTemp += Time.deltaTime;
        score = Mathf.RoundToInt(scoreTemp);

        if (inGameUIManager != null)
            inGameUIManager.CurrentScore.text = score.ToString();

        if (starPowerUp) return;

        if (!runTimer) return;
        if (gameSpeed == timeSpeedCap) return;
        if (gameSpeed > timeSpeedCap) { gameSpeed = timeSpeedCap; }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = timerCooldown;
            if (adjustedSpeed > timeSpeedCap) { adjustedSpeed = timeSpeedCap; }
            gameSpeed = adjustedSpeed;
            AdjustGameSpeed();
        }
        else
        {
            gameSpeedMultiplier += speedIncreaseRate * Time.deltaTime;
            adjustedSpeed = originalSpeed * gameSpeedMultiplier;
            if (adjustedSpeed > timeSpeedCap) { adjustedSpeed = timeSpeedCap; }
        }
    }

    private void AddCoin()
    {
        coins++;

        if (inGameUIManager)
        {
            inGameUIManager.CurrentCoins.text = coins.ToString();
        }
    }

    private void GameOver()
    {
        gameOver = true;
        runTimer = false;
        gameSpeed = 0;
        AdjustGameSpeed();

        SaveData.PlayerProfile.coins += coins;
        if (SaveData.PlayerProfile.highscore < score)
            SaveData.PlayerProfile.highscore = score;
        Save();
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

    private void StarPowerUp()
    {
        if (playerReferences == null)
            playerReferences = FindObjectOfType<PlayerReferences>();
        if (playerReferences == null) return;

        playerReferences.PlayerController.OnStarPowerUp?.Invoke();
        starPowerUp = true;
        invincible = true;
        gameSpeed = gameSpeed * 1.3f;
    }

    public void ReAddHealth()
    {
        StartCoroutine(IReAddHealth());
    }

    private IEnumerator IReAddHealth()
    {
        yield return new WaitForSeconds(reAddHealthTime);
        if (health > 0)
        {
            health++;
        }
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


    public void ResetStar()
    {
        starPowerUp = false;
        invincible = false;
        gameSpeed = gameSpeed / 1.3f;
    }

    public void RestartGame()
    {
        adjustedSpeed = originalSpeed;
        runTimer = true;
        gameOver = false;
        paused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
