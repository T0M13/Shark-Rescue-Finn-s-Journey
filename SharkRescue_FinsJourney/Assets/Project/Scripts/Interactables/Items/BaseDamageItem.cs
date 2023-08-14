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
        //Debug.Log("Damaging Player: " + Value);

        if (GameManager.Instance != null && !GameManager.Instance.Invincible)
        {
            GameManager.Instance.OnGetDamage?.Invoke(damageValue);
        }
        //GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }

    public void PlaySFX()
    {
        //if (AudioManager.instance)
        //AudioManager.instance.Play("damage");
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if ((other != null && other.gameObject.CompareTag("ChunkCatcher")) || (other != null && other.gameObject.CompareTag("Player")))
        {
            if ((other != null && other.gameObject.CompareTag("Player")))
            {
                if (!GameManager.Instance.Invincible)
                {
                    other.gameObject.GetComponent<PlayerReferences>().PlayerAnimator.SetTrigger("DamageTrigger");
                    other.gameObject.GetComponent<PlayerReferences>().PlayerInteractor.PlayerLightHitEffect();
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
                }

                other.gameObject.GetComponent<PlayerReferences>().PlayerInteractor.PlayerLightCollisionEffect();
                AudioManager.instance.Play("collision");
            }

            for (int i = 0; i < ItemSpawnerNew.Instance.ItemPrefabs.Count; i++)
            {
                if (powerUpType == ItemSpawnerNew.Instance.ItemPrefabs[i].powerUpType)
                {
                    gameObject.SetActive(false);
                    ItemSpawnerNew.Instance.ItemPrefabs[i].disabledItemList.Add(gameObject);
                    ItemSpawnerNew.Instance.ItemPrefabs[i].activeItemList.Remove(gameObject);
                }
            }
        }
    }
}
