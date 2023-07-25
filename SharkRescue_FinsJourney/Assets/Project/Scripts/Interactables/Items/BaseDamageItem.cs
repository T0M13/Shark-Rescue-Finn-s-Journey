using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDamageItem : BaseItem, IInteractable
{
    [Header("Damage Value")]
    [SerializeField] private int damageValue = 1;
    public int Value { get => damageValue; set => this.damageValue = value; }

    public void Interact()
    {
        Debug.Log("Damaging Player: " + Value);

        if (GameManager.instance != null)
        {
            GameManager.instance.OnGetDamage?.Invoke(damageValue);
        }
        //GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }

    public void PlaySFX()
    {
        //if (AudioManager.instance)
            //AudioManager.instance.Play("damage");
    }
}
