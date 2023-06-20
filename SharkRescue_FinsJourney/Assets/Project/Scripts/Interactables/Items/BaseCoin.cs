using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCoin : BaseItem, IInteractable
{
    [Header("Coin Value")]
    [SerializeField] private int value = 1;
    [Header("Coin Effect")]
    [SerializeField] private ParticleSystem effect;

    public int Value { get => value; set => this.value = value; }

    public void Interact()
    {
        Debug.Log("Adding Coins: " + Value);

        if (effect != null)
        {
            effect.gameObject.transform.SetParent(null);
            effect.Simulate(0.0f, true, true);
            effect.Play();
            effect.gameObject.GetComponent<VFXHelper>().ActivateVFX();
        }

        if (GameManager.instance != null)
            GameManager.instance.OnAddCoin?.Invoke();
        GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }
}
