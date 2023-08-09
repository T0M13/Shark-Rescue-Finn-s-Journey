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

    public void UpdateGameOverStats(int score, int coins)
    {
        gameOverScore.text = score.ToString();
        gameOverCoins.text = coins.ToString();
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
