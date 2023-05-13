using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCoin : BaseItem, IInteractable
{
    [Header("Coin Value")]
    [SerializeField] private int value = 1;
    public int Value { get => value; set => this.value = value; }

    public void Interact()
    {
        Debug.Log("Adding Coins: " + Value);

        if (GameManager.instance != null)
            GameManager.instance.OnAddCoin?.Invoke();
        GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }
}
