using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPowerUp : BaseItem, IInteractable
{
    public void Interact()
    {
        //Debug.Log("Speed Power Up Activated");

        if (GameManager.instance != null)
            GameManager.instance.OnStarPowerUp?.Invoke();
    }

    public void PlaySFX()
    {
        //if (AudioManager.instance)
            //AudioManager.instance.Play("speedPower");
    }

    protected override void Update()
    {
        base.Update();
        transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
    }
}
