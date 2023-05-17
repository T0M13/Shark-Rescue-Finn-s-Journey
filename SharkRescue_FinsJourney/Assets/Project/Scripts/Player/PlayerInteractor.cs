using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour
{

    [Header("Magnet")]
    [SerializeField] private bool magnetOn = false;
    [SerializeField] private float magnetRadius = 4.2f;
    [SerializeField] private float magnetPowerUpTime = 10f;
    [SerializeField] private float magnetCollectTime = 1.4f;
    [SerializeField] private GameObject magnetEffect;
    public Action OnMagnetPowerUp;

    private void OnEnable()
    {
        OnMagnetPowerUp += MagnetPowerUp;
    }

    private void OnDisable()
    {
        OnMagnetPowerUp -= MagnetPowerUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact();
        }
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
        if (!magnetOn) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, magnetRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<BaseCoin>())
            {
                collider.transform.position = Vector3.Lerp(collider.transform.position, transform.position, magnetCollectTime * Time.deltaTime);
            }
        }
    }
    #endregion

}
