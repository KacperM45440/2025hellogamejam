using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public List<StoreItem> currentInventory = new();
    public List<GameObject> spawnedItems = new();
    public GameObject itemPrefab;

    public void AddToInventory(StoreItem newItem)
    {
        currentInventory.Add(newItem);
    }

    public void SpawnItems()
    {
        foreach (StoreItem item in currentInventory)
        {
            GameObject newItem = Instantiate(itemPrefab);
            spawnedItems.Add(newItem);
        }
    }

    public void RemoveFromInventory(StoreItem removedItem)
    {
        currentInventory.Remove(removedItem);
    }

    public void DestroyItems()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return null;
        foreach (GameObject item in spawnedItems)
        {
            spawnedItems.Remove(item);
            Destroy(item);
        }
    }
}
