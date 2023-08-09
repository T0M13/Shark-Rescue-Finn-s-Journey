using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Collision")]
    [SerializeField] private GameObject collisionEffect;
    [SerializeField] private GameObject collisionEffectLight;
    [SerializeField] private Coroutine collisionLight;


    [Header("Magnet")]
    [SerializeField] private bool magnetOn = false;
    [SerializeField] private float magnetRadius = 4.2f;
    [SerializeField] private float magnetPowerUpTime = 10f;
    [SerializeField] private GameObject magnetEffect;
    [SerializeField] private List<GameObject> pulledObjects;
    public Action OnMagnetPowerUp;

    private void OnEnable()
    {
        OnMagnetPowerUp += MagnetPowerUp;
    }

    private void OnDisable()
    {
        OnMagnetPowerUp -= MagnetPowerUp;
    }

    public void PlayerGameOver()
    {
        collisionEffect.SetActive(true);
        collisionEffect.transform.SetParent(null);
        gameObject.SetActive(false);

    }

    public void PlayerLightCollisionEffect()
    {
        collisionLight = StartCoroutine(ResetCollisionEffect());
    }

    private IEnumerator ResetCollisionEffect()
    {
        collisionEffectLight.SetActive(true);
        yield return new WaitForSeconds(2f);
        collisionEffectLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact();
            interactable.PlaySFX();
        }

        if (other.gameObject.CompareTag("HardObstacle") && !GameManager.Instance.Invincible)
        {
            AudioManager.instance.Play("collision");

            //if (GameManager.Instance.Invincible) return;
            GameManager.Instance.OnGameOver?.Invoke();

            collisionEffect.SetActive(true);
            collisionEffect.transform.SetParent(null);

            //playerReferences.PlayerAnimator.SetBool("isGameOver", true);

            gameObject.SetActive(false);
        }

    }

    private void Awake()
    {
        if (playerReferences == null) playerReferences = GetComponent<PlayerReferences>();
    }

    private void Update()
    {
        MagnetLogic();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }


    #region Magnet Power Up

    /// <summary>
    /// Starts the Coroutine Magnet Power Up
    /// </summary>
    private void MagnetPowerUp()
    {
        StartCoroutine(MagnetPowerUpIE());
    }

    /// <summary>
    /// Activates Power Up for Magnet and after *magnetPowerUpTime* it deactivates it
    /// </summary>
    /// <returns></returns>
    private IEnumerator MagnetPowerUpIE()
    {
        magnetOn = true;
        if (magnetEffect != null)
            magnetEffect.SetActive(true);
        yield return new WaitForSeconds(magnetPowerUpTime);
        magnetOn = false;
        if (magnetEffect != null)
            magnetEffect.SetActive(false);
    }

    /// <summary>
    /// The logic so the Magnet Power Up works..
    /// </summary>
    private void MagnetLogic()
    {
        if (pulledObjects.Count > 0)
        {
            foreach (GameObject item in pulledObjects)
            {
                if (!item.activeInHierarchy) { pulledObjects.Remove(item); return; }
                item.transform.position = Vector3.Lerp(item.transform.position, transform.position, GameManager.Instance.GameSpeed * Time.deltaTime);
            }
        }

        if (!magnetOn) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, magnetRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<BaseCoin>())
            {
                if (pulledObjects.Contains(collider.gameObject)) return;
                pulledObjects.Add(collider.gameObject);
            }
        }
    }
    #endregion

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("HardObstacle"))
    //    {
    //        if (GameManager.instance.Invincible) return;
    //        GameManager.instance.OnGameOver?.Invoke();

    //        collisionEffect.SetActive(true);
    //        collisionEffect.transform.SetParent(null);

    //        //playerReferences.PlayerAnimator.SetBool("isGameOver", true);

    //        gameObject.SetActive(false);
    //    }
    //}
}
