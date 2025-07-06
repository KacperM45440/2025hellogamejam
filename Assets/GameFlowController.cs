using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StageManager;
public class GameFlowController : MonoBehaviour
{
    public StageManager stageManagerRef;
    public TabletController tabletControllerRef;
    public InventoryController inventoryControllerRef;
    public MoneyController moneyControllerRef;
    public ClientController clientControllerRef;
    public DialogueController DialogueControllerRef;
    public CameraController cameraControllerRef;
    public UIController UIControllerRef;
    public GameObject blackScreen;
    public TextMeshProUGUI dayCount;
    public Rope RopeRef;
    public bool RopeSpawned;
    public bool RopeTugged;
    public int currentDay = 0;

    private string controllerName = "GameFlowController";
    private List<string> controllerRequirements = new List<string>();

    private void Start()
    {
        stageManagerRef.SetCurrentGameStage(StageManager.GameStage.StartDay);
        SendOutRequestsToControllers();
        //inventoryScriptRef.SpawnItems(false);
        //StartCoroutine(SpawnClient());
    }

    private void SendOutRequestsToControllers()
    {
        controllerRequirements.Clear();
        StageManager.GameStage currentStage = stageManagerRef.GetCurrentGameStage();
        switch (currentStage)
        {
            case StageManager.GameStage.StartDay:
                currentDay++;
                //should also spawn items in the workshop
                controllerRequirements.Add(controllerName);
                WaitForNextStage(3f);
                break;
            case StageManager.GameStage.ClientEnterStore:
                controllerRequirements.Add(controllerName);
                WaitForNextStage(2f);
                controllerRequirements.Add(clientControllerRef.ControllerName);
                clientControllerRef.CreateNextClient();
                break;
            case StageManager.GameStage.ClientGreeting:
                controllerRequirements.Add(DialogueControllerRef.ControllerName);
                DialogueControllerRef.PrepareDialogue(currentStage);
                break;
            case StageManager.GameStage.Rope:
                controllerRequirements.Add(controllerName);
                SpawnRope();
                break;
            case StageManager.GameStage.ClientRequest:
                controllerRequirements.Add(DialogueControllerRef.ControllerName);
                DialogueControllerRef.PrepareDialogue(currentStage);
                break;
            case StageManager.GameStage.ClientWaitForGun:
                if(currentDay == 1)
                {
                    controllerRequirements.Add(cameraControllerRef.ControllerName);
                    cameraControllerRef.UnlockWorkshop();
                    //show arrow to workshop
                }
                controllerRequirements.Add(clientControllerRef.ControllerName);
                clientControllerRef.SetClientCanReceiveGun(true);
                break;
            case StageManager.GameStage.ClientGunReview:
                controllerRequirements.Add(DialogueControllerRef.ControllerName);
                DialogueControllerRef.PrepareDialogue(currentStage);
                break;
            case StageManager.GameStage.ClientLeaveStore:
                controllerRequirements.Add(clientControllerRef.ControllerName);
                clientControllerRef.ClientPaysThenLeaves();
                break;
            case StageManager.GameStage.Tablet:
                controllerRequirements.Add(tabletControllerRef.ControllerName);
                tabletControllerRef.PullOutTablet();
                break;
            case StageManager.GameStage.FinishDay:
                //Tutaj powinien by� warunek sprawdzaj�cy czy jest to ostatni dzie� gry, czy nie
                controllerRequirements.Add(UIControllerRef.ControllerName);
                UIControllerRef.FinishDay();
                break;
            default:
                break;
        }
    }

    //ta funkcja mog�aby te� przyjmowa� GameStage jako parametr, aby si� upewni�, �e nie s� przywo�ywane jakie� stare funkcje
    public void FinishRequirement(string controller)
    {
        if (controllerRequirements.Contains(controller))
        {
            controllerRequirements.Remove(controller);
        }
        else
        {
            Debug.LogWarning($"FinishRequirement: Controller '{controller}' not found in the list!");
        }

        if (controllerRequirements.Count <= 0)
        {
            Debug.Log($"{controllerName}: All requirements met. Proceeding to next stage.");
            this.FinishStage();
        }
    }

    //tutaj ify, kt�ry stage powinien by� odpalony
    public void FinishStage()
    {
        StageManager.GameStage currentStage = stageManagerRef.GetCurrentGameStage();
        switch (currentStage)
        {
            case GameStage.StartDay://Zako�czyli�my oczekiwanie na pocz�tku dnia
                currentStage = GameStage.ClientEnterStore;
                break;
            case GameStage.ClientEnterStore://Zako�czyli�my tworzenie i przyj�cie klienta do sklepu
                currentStage = GameStage.ClientGreeting;
                break;
            case GameStage.ClientGreeting://NPC si� przywita�
                if(currentDay == 1)
                {
                    currentStage = GameStage.Rope;
                }
                else
                {
                    currentStage = GameStage.ClientRequest;
                }
                break;
            case GameStage.Rope://Zako�czyli�my animacj� liny
                currentStage = GameStage.ClientRequest;
                break;
            case GameStage.ClientRequest:
                currentStage = GameStage.ClientWaitForGun;
                break;
            case GameStage.ClientWaitForGun:
                currentStage = GameStage.ClientGunReview;
                break;
            case GameStage.ClientGunReview:
                currentStage = GameStage.ClientLeaveStore;
                break;
            case GameStage.ClientLeaveStore:
                if(clientControllerRef.GetTodaysClientsCount() > 0)
                {
                    currentStage = GameStage.ClientEnterStore;
                }
                else
                {
                    currentStage = GameStage.Tablet;
                }
                break;
            case GameStage.Tablet:
                currentStage = GameStage.FinishDay;
                break;
            case GameStage.FinishDay:
                currentStage = GameStage.StartDay;
                break;
        }
        SendOutRequestsToControllers();
    }

    public void JumpDirectlyToStage(StageManager.GameStage stage)
    {
        controllerRequirements.Clear();
        stageManagerRef.SetCurrentGameStage(stage);
        SendOutRequestsToControllers();
    }

    //                stageManagerRef.SetCurrentGameStage(StageManager.GameStage.EnterStore);
    //clientControllerRef.CreateNextClient();

    public void WaitForNextStage(float time)
    {
        StartCoroutine(WaitForSeconds(time));
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        Debug.Log($"{controllerName}: Waiting for {seconds} seconds.");
        yield return new WaitForSeconds(seconds);
        Debug.Log($"{controllerName}: Wait completed. Proceeding to next stage.");
        this.FinishRequirement(controllerName);
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
        FinishRequirement("GameFlowController");
        //DialogueControllerRef.currentSubdialogue = 0;
        //DialogueControllerRef.ProgressStage();
        //DialogueControllerRef.ProgressDialogue();
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
