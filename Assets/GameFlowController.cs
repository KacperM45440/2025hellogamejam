using System.Collections;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    public InventoryScript inventoryScriptRef;
    public MoneyController moneyControllerRef;
    public ClientController clientControllerRef;
    private void Start()
    {
        //Inicjalizacja rêki
        inventoryScriptRef.SpawnItems(false);
        StartCoroutine(SpawnClient());
    }

    private IEnumerator SpawnClient()
    {
        yield return new WaitForSeconds(5f);
        clientControllerRef.CreateNextClient();
    }

    public void SpawnRope()
    {
        Debug.Log("Spawn Rope");
    }

    public void RopeWasTugged()
    {
        Debug.Log("Rope was tugged, change store look.");
    }
}
