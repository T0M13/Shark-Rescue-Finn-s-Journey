using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private Canvas ingameCanvas;
    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI currentCoins;

    public TextMeshProUGUI CurrentScore { get => currentScore; set => currentScore = value; }
    public TextMeshProUGUI CurrentCoins { get => currentCoins; set => currentCoins = value; }
    public Transform GameOverPanel { get => gameOverPanel; set => gameOverPanel = value; }
}
