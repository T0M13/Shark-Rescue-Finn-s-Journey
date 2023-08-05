using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPowerUp : BaseItem, IInteractable
{
    public void Interact()
    {
        //Debug.Log("Magnet Power Up Activated");

        if (GameManager.Instance != null)
            GameManager.Instance.OnMagnetPowerUp?.Invoke();
    }

    public void PlaySFX()
    {
        if (AudioManager.instance)
            AudioManager.instance.Play("magnet");
    }
    protected override void Update()
    {
        base.Update();
        transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
    }
}
