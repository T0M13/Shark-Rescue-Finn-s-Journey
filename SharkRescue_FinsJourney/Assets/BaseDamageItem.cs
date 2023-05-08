using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDamageItem : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform meshTransform;
    [Header("Coin Value")]
    [SerializeField] private int damageValue = 1;
    [Header("Move Speed")]
    [SerializeField] private int moveSpeed = 1;
    [Header("Rotation Speed")]
    [SerializeField] private int rotationSpeedMin = 100;
    [SerializeField] private int rotationSpeedMax = 150;
    [SerializeField] private int rotationSpeed;

    public int MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public int Value { get => damageValue; set => this.damageValue = value; }
    public int RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    protected void Start()
    {
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
    }

    public void Interact()
    {
        Debug.Log("Damaging Player: " + Value);

        if (GameManager.instance != null)
            GameManager.instance.OnGetDamage?.Invoke(damageValue);
        GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
    }

    private void Update()
    {
        transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        meshTransform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Deactivater>())
        {
            GameManager.instance.OnDeactivateGObject?.Invoke(gameObject);
        }
    }
}
