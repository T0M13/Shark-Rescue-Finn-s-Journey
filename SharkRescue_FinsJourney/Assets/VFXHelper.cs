using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHelper : MonoBehaviour
{
    [Header("References")]
    public BaseItem parent;
    [Header("Settings")]
    public bool deactivate = true;
    public bool move = true;
    private bool moveTemp;

    public void ActivateVFX()
    {
        moveTemp = true;
        if (!deactivate)
            StartCoroutine(Kill());
        else
            StartCoroutine(Deactivate(parent.transform));

    }

    private void Update()
    {
        if (move && moveTemp)
        {
            transform.Translate(Vector3.back * parent.MoveSpeed * Time.deltaTime);
        }
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }

    IEnumerator Deactivate(Transform parent)
    {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
        moveTemp = false;
        gameObject.transform.SetParent(parent);


    }
}
