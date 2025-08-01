using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameFlowController FlowControllerRef;

    public List<GameObject> startingInventory = new();
    public List<GameObject> currentInventory = new();
    public List<GameObject> spawnedItems = new();

    [SerializeField] private Transform backupFrameSpawnPoint;
    [SerializeField] private GameObject defaultGunFrame;

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
        float yPos = 0f;
        foreach (GameObject item in currentInventory)
        {
            GameObject newItem = Instantiate(item);
            newItem.transform.position = CraftingMgr.Instance.GetItemSpawnPosition() + (new Vector3(0, (yPos++) / 50));
            spawnedItems.Add(newItem);
        }
        CheckFrameExistenceAndSpawnIfNeeded();
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

    public void RemoveFromInventoryByName(string removedItemName)
    {
        for (var i = 0; i < currentInventory.Count; i++)
        {
            if (currentInventory[i].GetComponent<Item>().name == removedItemName)
            {
                currentInventory.RemoveAt(i);
                break;
            }
        }
    }

    public void CheckFrameExistenceAndSpawnIfNeeded()
    {
        foreach (GameObject item in currentInventory)
        {
            if (item.GetComponent<Item>().itemType == ItemType.FRAME)
            {
                return;
            }
        }
        AddToInventory(defaultGunFrame);
        GameObject newItem = Instantiate(defaultGunFrame);
        newItem.transform.position = backupFrameSpawnPoint.position;
        spawnedItems.Add(newItem);
    }

    public void DestroyItems()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(5f);
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();
        FlowControllerRef.FinishRequirement(controllerName);
    }
}
