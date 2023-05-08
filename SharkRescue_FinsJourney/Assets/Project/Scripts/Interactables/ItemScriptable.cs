using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Items/Item")]
public class ItemScriptable : ScriptableObject
{
    public GameObject prefab;
    public float chanceToSpawn;

}
