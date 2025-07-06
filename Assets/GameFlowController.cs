using System.Collections;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    public InventoryScript inventoryScriptRef;
    public MoneyController moneyControllerRef;
    public ClientController clientControllerRef;
    public DialogueController DialogueControllerRef;
    public bool RopeTugged;
    
    private void Start()
    {
        //Inicjalizacja rêki
        inventoryScriptRef.SpawnItems(false);
        StartCoroutine(SpawnClient());
    }

    private IEnumerator SpawnClient()
    {
        yield return new WaitForSeconds(0.5f);
        clientControllerRef.CreateNextClient();
    }

    public void SpawnRope()
    {
        Debug.Log("Spawn Rope");
    }

    public void RopeWasTugged()
    {
        RopeTugged = true;
        Debug.Log("Rope was tugged, change store look.");
        DialogueControllerRef.ProgressStage();
        DialogueControllerRef.ProgressDialogue();
    }
}
