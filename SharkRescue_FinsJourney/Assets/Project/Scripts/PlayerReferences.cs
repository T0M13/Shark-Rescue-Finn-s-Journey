using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;

    public PlayerController PlayerController { get => playerController; set => playerController = value; }
    public PlayerInteractor PlayerInteractor { get => playerInteractor; set => playerInteractor = value; }

    private void Awake()
    {
        if (PlayerController == null)
            PlayerController = GetComponent<PlayerController>();
        if (PlayerInteractor == null)
            PlayerInteractor = GetComponent<PlayerInteractor>();
    }
}
