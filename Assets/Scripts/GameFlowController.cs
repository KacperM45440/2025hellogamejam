using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StageManager;
public class GameFlowController : MonoBehaviour
{
    [Header("Set Starting Day and Stage:")]
    [Range(1, 3)]
    public int currentDay = 1;
    public GameStage startStage = GameStage.StartDay;

    [Space(20)]
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
    public GameObject tutorialArrowRef;
    public bool RopeSpawned;
    public bool RopeTugged;

    public int lastDay = 3;

    private string controllerName = "GameFlowController";
    private List<string> controllerRequirements = new List<string>();

    private void Start()
    {
        stageManagerRef.SetCurrentGameStage(startStage);
        SendOutRequestsToControllers();
    }

    private void SendOutRequestsToControllers()
    {
        controllerRequirements.Clear();
        StageManager.GameStage currentStage = stageManagerRef.GetCurrentGameStage();
        switch (currentStage)
        {
            case StageManager.GameStage.StartDay:
                controllerRequirements.Add(inventoryControllerRef.ControllerName);
                inventoryControllerRef.SpawnAllItemsFromStorage();

                controllerRequirements.Add(controllerName);
                WaitForNextStage(3f);
                break;
            case StageManager.GameStage.ClientEnterStore:
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
                    //controllerRequirements.Add(cameraControllerRef.ControllerName);//dodawanie tego do requirements jest zbêdne
                    cameraControllerRef.UnlockWorkshop();
                    tutorialArrowRef.SetActive(true);
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
                controllerRequirements.Add(inventoryControllerRef.ControllerName);
                inventoryControllerRef.DestroyItems();
                controllerRequirements.Add(UIControllerRef.ControllerName);
                UIControllerRef.FinishDay();
                break;
            default:
                break;
        }
    }

    //ta funkcja mog³aby te¿ przyjmowaæ GameStage jako parametr, aby siê upewniæ, ¿e nie s¹ przywo³ywane jakieœ stare funkcje
    public void FinishRequirement(string controller)
    {
        StartCoroutine(FinishRequirementAsync(controller));
    }

    private IEnumerator FinishRequirementAsync(string controller)
    {
        yield return null;
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
            FinishStage();
        }
    }

    //tutaj ify, który stage powinien byæ odpalony
    public void FinishStage()
    {
        StageManager.GameStage currentStage = stageManagerRef.GetCurrentGameStage();
        switch (currentStage)
        {
            case GameStage.StartDay://Zakoñczyliœmy oczekiwanie na pocz¹tku dnia
                currentStage = GameStage.ClientEnterStore;
                break;
            case GameStage.ClientEnterStore://Zakoñczyliœmy tworzenie i przyjœcie klienta do sklepu
                currentStage = GameStage.ClientGreeting;
                break;
            case GameStage.ClientGreeting://NPC siê przywita³
                if(currentDay == 1)
                {
                    currentStage = GameStage.Rope;
                }
                else
                {
                    currentStage = GameStage.ClientRequest;
                }
                break;
            case GameStage.Rope://Zakoñczyliœmy animacjê liny
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
                    if(currentDay == lastDay)
                    {
                        UIControllerRef.EndGameBlackScreenAnimation();
                    }
                    else
                    {
                        currentStage = GameStage.Tablet;
                    }
                    currentStage = GameStage.Tablet;
                }
                break;
            case GameStage.Tablet:
                currentStage = GameStage.FinishDay;
                break;
            case GameStage.FinishDay:
                currentStage = GameStage.StartDay;
                currentDay++;
                break;
        }
        stageManagerRef.SetCurrentGameStage(currentStage);
        SendOutRequestsToControllers();
    }

    public void JumpDirectlyToStage(StageManager.GameStage stage)
    {
        controllerRequirements.Clear();
        stageManagerRef.SetCurrentGameStage(stage);
        SendOutRequestsToControllers();
    }

    public void WaitForNextStage(float time)
    {
        StartCoroutine(WaitForSeconds(time));
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.FinishRequirement(controllerName);
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
        yield return new WaitForSeconds(5f);
        FinishRequirement("GameFlowController");
    }

    private IEnumerator DisableAnim()
    {
        yield return new WaitForSeconds(1f);
        RopeRef.gameObject.GetComponent<Animator>().enabled = false;
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
