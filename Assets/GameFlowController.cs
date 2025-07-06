using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameFlowController : MonoBehaviour
{
    public TabletController tabletControllerRef;
    public InventoryScript inventoryScriptRef;
    public MoneyController moneyControllerRef;
    public ClientController clientControllerRef;
    public DialogueController DialogueControllerRef;
    public GameObject blackScreen;
    public TextMeshProUGUI dayCount;
    public Rope RopeRef;
    public bool RopeSpawned;
    public bool RopeTugged;
    public int currentDay = 1;
    
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

    public void ShowTablet()
    {
        tabletControllerRef.PullOutTablet();
    }

    public void EndDay()
    {
        StartCoroutine(EndDayAnim());
    }

    private IEnumerator EndDayAnim()
    {
        blackScreen.SetActive(true);
        currentDay++;
        dayCount.text = "Day: "+ currentDay.ToString();
        yield return new WaitForSeconds(1);
        dayCount.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        blackScreen.SetActive(false);
        dayCount.gameObject.SetActive(false);
        StartNextDay();
    }

    public void StartNextDay()
    {

    }

    public void EndGame()
    {
        bool won = false;
        if (moneyControllerRef.HasEnoughMoneyInJar())
        {
            won = true;
        }
        PlayerPrefs.SetInt("GameWin", Convert.ToInt32(won));
        SceneManager.LoadScene(3);
    }
}
