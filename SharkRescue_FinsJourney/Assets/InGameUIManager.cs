using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private Canvas ingameCanvas;
    [SerializeField] private TextMeshProUGUI currentScore;

    public TextMeshProUGUI CurrentScore { get => currentScore; set => currentScore = value; }
}
