using System.Collections;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    public InventoryScript inventoryScriptRef;
    public MoneyController moneyControllerRef;
    public ClientController clientControllerRef;
    public DialogueController DialogueControllerRef;
    public Rope RopeRef;
    public bool RopeSpawned;
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
        RopeSpawned = true;
        RopeRef.gameObject.GetComponent<Animator>().SetTrigger("Tug");
        StartCoroutine(DisableAnim());
    }

    public void RopeWasTugged()
    {
        RopeTugged = true;
        StartCoroutine(StoreChangeAnim());
    }
    
    private IEnumerator StoreChangeAnim()
    {
        yield return new WaitForSeconds(3f);
        DialogueControllerRef.currentSubdialogue = 0;
        DialogueControllerRef.ProgressStage();
        DialogueControllerRef.ProgressDialogue();
    }

    private IEnumerator DisableAnim()
    {
        yield return new WaitForSeconds(1f);
        RopeRef.gameObject.GetComponent<Animator>().enabled = false;
    }
}
