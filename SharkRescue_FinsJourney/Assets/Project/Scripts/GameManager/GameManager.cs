using System;
using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnvironmentType;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;
    [SerializeField] private InGameUIManager inGameUIManager;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private ItemSpawnerNew itemSpawner;
    [SerializeField] private ObstacleManager obstacleManager;
    [SerializeField] private AudioManager audioManager;
    [Header("Game Environment")]
    [SerializeField] private EEnvironmentType eEnvironmentTyp;
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
    [SerializeField] private int gameDifficulty = 1;
    [SerializeField] private int timerChangeGameDifficutly = 30;
    [SerializeField] private bool changeGameDifficutly = true;
    [SerializeField] private bool changeEnvironment = true;
    [SerializeField] private int minChangeEnvironmentTime = 60;
    [SerializeField] private int maxChangeEnvironmentTime = 120;
    [SerializeField, Range(0, 100)] private int changeEnvironmentChance = 60;
    [SerializeField] private List<EEnvironmentType> environmentTypeList = new List<EEnvironmentType>();
    [Header("Player Stats")]
    [SerializeField] private int health = 2;
    [SerializeField] private int healthMax = 2;
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
    public int GameDifficutly { get => gameDifficulty; }
    public bool Invincible { get => invincible; set => invincible = value; }
    public int Health { get => health; set => health = value; }
    public int Score { get => score; set => score = value; }
    public int Coins { get => coins; set => coins = value; }

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
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
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


        Application.targetFrameRate = 60;

        ChangeEnvironmentSetUp();
        StartCoroutine(ChangeEnvironment());
    }

    private void Start()
    {
        if (audioManager)
        {
            audioManager.Play("bubbles");
            audioManager.LoadVolumes();
        }

        StartCoroutine(ChangeGameDifficutly(timerChangeGameDifficutly));

    }

    private void Update()
    {
        if (gameOver) return;

        scoreTemp += Time.deltaTime;
        Score = Mathf.RoundToInt(scoreTemp);

        if (inGameUIManager != null)
            inGameUIManager.CurrentScore.text = Score.ToString();

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
        Coins++;

        if (inGameUIManager)
        {
            inGameUIManager.CurrentCoins.text = Coins.ToString();
        }
    }

    private void GameOver()
    {
        gameOver = true;
        runTimer = false;
        gameSpeed = 0;
        AdjustGameSpeed();

        if (inGameUIManager)
        {
            inGameUIManager.UpdateGameOverStats(Score, Coins);
            inGameUIManager.GameOverPanel.gameObject.SetActive(true);
            inGameUIManager.HudPanel.gameObject.SetActive(false);
        }

        SaveData.PlayerProfile.coins += Coins;
        if (SaveData.PlayerProfile.highscore < Score)
            SaveData.PlayerProfile.highscore = Score;
        Save();
    }

    public void GetDamage(int damageValue)
    {
        if (invincible) return;

        Health -= damageValue;

        ReAddHealth();


        if (Health <= 0)
        {
            GameOver();
            playerReferences.PlayerInteractor.PlayerGameOver();
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

        AudioManager.instance.Play("star");
        AudioManager.instance.audioSource.volume = 0;

        starPowerUp = true;
        Invincible = true;
        gameSpeed = gameSpeed * 1.3f;
    }

    public void ReAddHealth()
    {
        if (Health < healthMax)
            StartCoroutine(IReAddHealth());
    }

    private IEnumerator IReAddHealth()
    {
        yield return new WaitForSeconds(reAddHealthTime);
        if (Health > 0 && Health < healthMax)
        {
            Health++;
            OnReAddHealth?.Invoke();
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
        Invincible = false;
        gameSpeed = gameSpeed / 1.3f;
        AudioManager.instance.Stop("star");
        AudioManager.instance.audioSource.volume = 1;

    }

    public void RestartGame()
    {
        adjustedSpeed = originalSpeed;
        runTimer = true;
        gameOver = false;
        paused = false;
        Time.timeScale = 1;
        if (inGameUIManager) { inGameUIManager.GameOverPanel.gameObject.SetActive(false); }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void StartGame()
    {
        runTimer = true;
        gameOver = false;
        paused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMainMenu()
    {
        runTimer = false;
        gameOver = true;
        paused = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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

    private IEnumerator ChangeGameDifficutly(int timerChangeGameDifficutly)
    {
        while (changeGameDifficutly)
        {
            yield return new WaitForSeconds(timerChangeGameDifficutly);

            gameDifficulty++;
            ParticleManager.Instance.AdjustParticleSpeed(gameDifficulty);

            if (gameDifficulty == 10)
            {
                changeGameDifficutly = false;
            }
        }
    }

    private void ChangeEnvironmentSetUp()
    {
        environmentTypeList.Clear();

        for (int i = 1; i < Enum.GetNames(typeof(EEnvironmentType)).Length; i++)
        {
            environmentTypeList.Add((EEnvironmentType)i);
        }
    }

    private IEnumerator ChangeEnvironment()
    {
        int randomChance;
        int randomTime;
        int randomBiom;

        while (!gameOver && changeEnvironment)
        {
            //Debug.Log("ChangeEnvironment       while");
            randomChance = UnityEngine.Random.Range(0, 100);
            randomTime = UnityEngine.Random.Range(minChangeEnvironmentTime, maxChangeEnvironmentTime);
            //Debug.Log("randomChance " + randomChance);

            if (randomChance > changeEnvironmentChance && eEnvironmentTyp != EEnvironmentType.NONE && true) //Probability of changing the biome
            {
                //Debug.Log("NoBiomChange");
                yield return new WaitForSeconds(randomTime);
            }

            List<EEnvironmentType> environmentTypeListtemp = new(environmentTypeList);
            randomBiom = UnityEngine.Random.Range(0, environmentTypeListtemp.Count);

            for (int i = 0; i < 2; i++)
            {
                //Debug.Log("randomBiom " + randomBiom);
                if (environmentTypeListtemp[randomBiom] != eEnvironmentTyp)
                {
                    eEnvironmentTyp = environmentTypeListtemp[randomBiom];
                    //Debug.Log("eEnvironmentTyp " + eEnvironmentTyp);
                    break;
                }
                else
                {
                    environmentTypeListtemp.RemoveAt(randomBiom);
                    randomBiom = UnityEngine.Random.Range(0, environmentTypeListtemp.Count);
                }
            }

            yield return new WaitForSeconds(randomTime);
        }
    }
}
