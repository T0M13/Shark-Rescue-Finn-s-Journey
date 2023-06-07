using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerUpTypes;

public class BaseItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform meshTransform;
    [Header("Move Speed")]
    [SerializeField] private int moveSpeed = 1;
    [Header("Rotation Speed")]
    [SerializeField] private int rotationSpeedMin = 100;
    [SerializeField] private int rotationSpeedMax = 150;
    [SerializeField] private int rotationSpeed = 100;

    [SerializeField] private PowerUpType powerUpType;
    public int MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public int RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    protected void Start()
    {
        if (meshTransform == null)
            meshTransform = transform.GetChild(0);
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
    }

    protected void OnValidate()
    {
        if (meshTransform == null)
            meshTransform = transform.GetChild(0);
    }

    protected void Update()
    {
        transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        meshTransform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("ChunkCatcher"))
        {
            for (int i = 0; i < ItemSpawnerNew.Instance.ItemPrefabs.Count; i++)
            {
                if(powerUpType == ItemSpawnerNew.Instance.ItemPrefabs[i].powerUpType)
                {
                    gameObject.SetActive(false);
                    ItemSpawnerNew.Instance.ItemPrefabs[i].disabledItemList.Add(gameObject);
                    ItemSpawnerNew.Instance.ItemPrefabs[i].activeItemList.Remove(gameObject);
                }
            }
        }
    }

}
