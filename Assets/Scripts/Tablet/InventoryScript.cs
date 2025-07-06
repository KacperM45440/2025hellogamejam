using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public List<GameObject> startingInventory = new();
    public List<GameObject> currentInventory = new();
    public List<GameObject> spawnedItems = new();

    private void Start()
    {
        foreach (GameObject item in startingInventory)
        {
            currentInventory.Add(item);
        }
        //SpawnItems(true);

    }

    public void AddToInventory(GameObject newItem)
    {
        currentInventory.Add(newItem);
    }

    public void SpawnItems(bool visible)
    {
        foreach (GameObject item in currentInventory)
        {
            GameObject newItem = Instantiate(item);
            spawnedItems.Add(newItem);
            if (!visible)
            {
                newItem.SetActive(false);
            }
        }
    }

    public void MakeItemsVisible()
    {
        foreach (GameObject item in spawnedItems)
        {
            item.SetActive(true);
        }
    }

    public void RemoveFromInventory(GameObject removedItem)
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
