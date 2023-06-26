using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPowerUp : BaseItem, IInteractable
{
    public void Interact()
    {
        //Debug.Log("Magnet Power Up Activated");

        if (GameManager.instance != null)
            GameManager.instance.OnMagnetPowerUp?.Invoke();
        GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }
}
