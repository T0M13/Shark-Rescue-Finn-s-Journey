using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : BaseItem, IInteractable
{
    public void Interact()
    {
        //Debug.Log("Speed Power Up Activated");

        if (GameManager.instance != null)
            GameManager.instance.OnSpeedPowerUp?.Invoke();
        GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }

    public void PlaySFX()
    {
        //if (AudioManager.instance)
            //AudioManager.instance.Play("speedPower");
    }
}
