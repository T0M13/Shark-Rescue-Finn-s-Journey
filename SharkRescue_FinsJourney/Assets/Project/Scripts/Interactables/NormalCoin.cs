using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCoin : MonoBehaviour, IInteractable
{
    private ItemSpawner parentSpawner;
    [SerializeField] private int value = 1;
    [SerializeField] private int moveSpeed = 1;

    public void SetParent(ItemSpawner spawner)
    {
        parentSpawner = spawner;
    }

    public void Interact()
    {
        Debug.Log("Adding Coins: " + value);

        if (GameManager.instance != null)
            GameManager.instance.OnAddCoin?.Invoke();
        Deactivate();
    }

    private void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
    }

    public void Deactivate()
    {
        parentSpawner.DeactivateObject(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Deactivater>())
        {
            Deactivate();
        }
    }
}
