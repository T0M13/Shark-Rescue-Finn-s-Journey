using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCoin : MonoBehaviour, IInteractable
{
    [SerializeField] private int value = 1;

    public void Interact()
    {
        Debug.Log("Add Coins: " + value);
    }

    //Deactivate --> Object Pool
}
