using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXKiller : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }
}
