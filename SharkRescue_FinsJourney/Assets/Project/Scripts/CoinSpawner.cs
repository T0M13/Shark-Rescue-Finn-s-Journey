using UnityEngine;

public class CoinSpawner : ItemSpawner
{
    protected override void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject clone = Instantiate(prefab, transform);
            clone.SetActive(false);
            clone.GetComponent<BaseCoin>().SetParent(this);
            pooledObjects.Add(clone);
        }
    }
}
