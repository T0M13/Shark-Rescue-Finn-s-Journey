using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private Canvas ingameCanvas;
    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Transform hudPanel;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI currentCoins;
    [SerializeField] private Transform healthPlaceholder;
    [SerializeField] private Sprite healthImagePrefab;
    [SerializeField] private List<Image> healthPoints;
    [SerializeField] private TextMeshProUGUI gameOverScore;
    [SerializeField] private TextMeshProUGUI gameOverCoins;
    [SerializeField] private TextMeshProUGUI gameOverAllCoins;
    [SerializeField] private TextMeshProUGUI gameOverHighscore;


    private int score;
    private int highscore;
    private int coins;
    private int allcoins;

    public TextMeshProUGUI CurrentScore { get => currentScore; set => currentScore = value; }
    public TextMeshProUGUI CurrentCoins { get => currentCoins; set => currentCoins = value; }
    public Transform GameOverPanel { get => gameOverPanel; set => gameOverPanel = value; }
    public List<Image> HealthPoints { get => healthPoints; set => healthPoints = value; }
    public Transform HudPanel { get => hudPanel; set => hudPanel = value; }

    private void OnEnable()
    {
        GameManager.Instance.OnGetDamage += UpdateHealthP;
        GameManager.Instance.OnReAddHealth += UpdateHealthP;

    }

    private void OnDisable()
    {
        GameManager.Instance.OnGetDamage -= UpdateHealthP;
        GameManager.Instance.OnReAddHealth -= UpdateHealthP;

    }

    private void Start()
    {
        SetUpHealthPoints();
    }

    public void UpdateGameOverStats()
    {
        gameOverScore.text = this.score.ToString();
        gameOverCoins.text = "+" + this.coins.ToString();
        gameOverAllCoins.text = this.allcoins.ToString();
        gameOverHighscore.text = this.highscore.ToString();
    }

    public void GetStats(int score, int highscore, int coins, int allcoins)
    {
        this.score = score;
        this.highscore = highscore;
        this.coins = coins;
        this.allcoins = allcoins;
    }

    private void SetUpHealthPoints()
    {
        if (GameManager.Instance)
        {
            int h = GameManager.Instance.Health;

            for (int i = 0; i < h; i++)
            {
                GameObject cloneObject = new GameObject("HealthImage" + i);
                RectTransform clone = cloneObject.AddComponent<RectTransform>();

                clone.SetParent(healthPlaceholder, false);

                Image imageComponent = cloneObject.AddComponent<Image>();

                imageComponent.sprite = healthImagePrefab;

                healthPoints.Add(imageComponent);
            }

        }
    }

    private void UpdateHealthP(int h)
    {

        foreach (Image item in healthPoints)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.Instance.Health; i++)
        {
            healthPoints[i].gameObject.SetActive(true);
        }
    }

    private void UpdateHealthP()
    {
        foreach (Image item in healthPoints)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.Instance.Health; i++)
        {
            healthPoints[i].gameObject.SetActive(true);
        }
    }


}
