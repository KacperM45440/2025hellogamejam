using System.Collections.Generic;
using UnityEngine;
using GameStage = StageManager.GameStage;

public class DialogueController : Singleton<DialogueController>
{
    public GameFlowController FlowControllerRef;
    public InventoryController inventoryRef;
    public StageManager StageManagerRef;
    public DialogueData DialogueDataRef;
    public ClientController ClientRef;
    public BubbleController BubbleRef;

    [HideInInspector] public Dialogue mainDialogue;
    [HideInInspector] public List<string> currentDialogue;
    [HideInInspector] public int currentSubdialogue = 0;
    
    [HideInInspector] public List<List<string>> DialogueGreetings;
    [HideInInspector] public List<List<string>> DialogueRequests;
    [HideInInspector] public List<List<string>> DialogueResponsesGood;
    [HideInInspector] public List<List<string>> DialogueResponsesAverage;
    [HideInInspector] public List<List<string>> DialogueResponsesBad;

    private string controllerName = "DialogueController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

    public void Start()
    {
        InitializeDialogue();
        ChangeMainDialogue(0);
    }

    public void InitializeDialogue()
    {
        DialogueGreetings = DialogueDataRef.CreateDialogueGreeting();
        DialogueRequests = DialogueDataRef.CreateDialogueRequest();
        DialogueResponsesGood = DialogueDataRef.CreateResponseGood();
        DialogueResponsesAverage = DialogueDataRef.CreateResponseAverage();
        DialogueResponsesBad = DialogueDataRef.CreateResponseBad();
    }

    public void ChangeMainDialogue(int index)
    {
        mainDialogue = new Dialogue();

        mainDialogue.Greeting = DialogueGreetings[index];
        mainDialogue.Request = DialogueRequests[index];
        mainDialogue.ResponseGood = DialogueResponsesGood[index];
        mainDialogue.ResponseAverage = DialogueResponsesAverage[index];
        mainDialogue.ResponseBad = DialogueResponsesBad[index];
    }

    public void ProgressDialogue()
    {
        if (currentDialogue.Count == 0)
        {
            return;
        }

        //Debug.Log("sub: " + currentSubdialogue + "count: " + currentDialogue.Count);

        if (currentSubdialogue <= currentDialogue.Count - 1)
        {
            BubbleRef.NextText(currentSubdialogue, ClientRef.CurrentClient.ClientName, currentDialogue[currentSubdialogue]);
            currentSubdialogue++;
        }
        else
        {
            ClearDialogueQueue();
            FlowControllerRef.FinishRequirement(controllerName);
            //ProgressStage();
        }
    }

    private void ClearDialogueQueue()
    {
        currentDialogue.Clear();
        currentSubdialogue = 0;
        BubbleRef.ClearText();
    }

    public void PrepareDialogue(GameStage stage)
    {
        ClearDialogueQueue();
        switch (stage)
        {
            case GameStage.ClientGreeting:
                currentDialogue = mainDialogue.Greeting;
                break;
            case GameStage.ClientRequest:
                currentDialogue = mainDialogue.Request;
                break;
            case GameStage.ClientGunReview:
                ClientController.ClientMood mood = ClientRef.GetCurrentClientMood();
                switch (mood)
                {
                    case ClientController.ClientMood.Good:
                        currentDialogue = mainDialogue.ResponseGood;
                        break;
                    case ClientController.ClientMood.Neutral:
                        currentDialogue = mainDialogue.ResponseAverage;
                        break;
                    case ClientController.ClientMood.Bad:
                        currentDialogue = mainDialogue.ResponseBad;
                        break;
                    default:
                        break;
                }
                break;
        }
        ProgressDialogue();
    }

    public class Dialogue
    {
        public List<string> Greeting;
        public List<string> Request;
        public List<string> ResponseGood;
        public List<string> ResponseAverage;
        public List<string> ResponseBad;
    }
}
