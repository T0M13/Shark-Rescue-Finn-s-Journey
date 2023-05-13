using System;
using System.Collections;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Magnet")]
    [SerializeField] private bool magnetOn = false;
    [SerializeField] private float magnetRadius = 4.2f;
    [SerializeField] private float magnetPowerUpTime = 10f;
    [SerializeField] private float magnetCollectTime = 1.4f;

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

    private void MagnetPowerUp()
    {
        StartCoroutine(MagnetPowerUpIE());
    }

    private IEnumerator MagnetPowerUpIE()
    {
        magnetOn = true;
        yield return new WaitForSeconds(magnetPowerUpTime);
        magnetOn = false;
    }

    private void Update()
    {
        if (!magnetOn) return;
        MagnetLogic();
    }

    private void MagnetLogic()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, magnetRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<BaseCoin>())
            {
                collider.transform.position = Vector3.Lerp(collider.transform.position, transform.position, magnetCollectTime * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
