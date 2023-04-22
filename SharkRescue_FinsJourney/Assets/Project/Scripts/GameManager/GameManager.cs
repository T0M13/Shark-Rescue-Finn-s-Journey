using System;
using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Game Positions")]
    [SerializeField] private Vector3 startPosition;
    [Header("Player Stats")]
    [SerializeField] private int coins;
    [Header("Save/Load")]
    [SerializeField] private SaveComponent saveBehaviour;
    [SerializeField] private LoadComponent loadBehaviour;
    [Header("Saved Stats")]
    [SerializeField] private int playerProfileCoins;

    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }

    public Action OnAddCoin;
    public Action OnSave;
    public Action OnLoad;
    public Action OnGameOver;

    private void OnEnable()
    {
        OnAddCoin += AddCoin;
        OnSave += Save;
        OnLoad += Load;
        OnGameOver += GameOver; 
    }

    private void OnDisable()
    {
        OnAddCoin -= AddCoin;
        OnSave -= Save;
        OnLoad -= Load;
        OnGameOver -= GameOver;
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
        //GameOver
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
