using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameFlowController FlowControllerRef;

    public List<GameObject> startingInventory = new();
    public List<GameObject> currentInventory = new();
    public List<GameObject> spawnedItems = new();

    private string controllerName = "InventoryController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

    private void Start()
    {
        foreach (GameObject item in startingInventory)
        {
            currentInventory.Add(item);
        }
    }

    public void AddToInventory(GameObject newItem)
    {
        currentInventory.Add(newItem);
    }

    public void SpawnAllItemsFromStorage()
    {
        StartCoroutine(SpawnItemsAsync());
    }

    private IEnumerator SpawnItemsAsync()
    {
        yield return null;
        foreach (GameObject item in currentInventory)
        {
            GameObject newItem = Instantiate(item);
            spawnedItems.Add(newItem);
        }
        FlowControllerRef.FinishRequirement(controllerName);
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
