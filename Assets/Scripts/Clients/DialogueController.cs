using System.Collections.Generic;
using UnityEngine;
using GameStage = StageManager.GameStage;

public class DialogueController : MonoBehaviour
{
    public GameFlowController FlowControllerRef;
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


    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProgressDialogue();
        }
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

        if (currentSubdialogue <= currentDialogue.Count - 1)
        {
            BubbleRef.NextText(currentSubdialogue, ClientRef.CurrentClient.ClientName, currentDialogue[currentSubdialogue]);
            currentSubdialogue++;
        }
        else
        {
            BubbleRef.ClearText();

            if (ClientRef.CurrentClientInt == 1)
            {
                if (!FlowControllerRef.RopeSpawned)
                {
                    FlowControllerRef.SpawnRope();
                }              

                return;
            }

            ProgressStage();
        }
    }

    public void WeaponFeedback(string feedback) // string is a placeholder, should be enum
    {
        StageManagerRef.SetCurrentGameStage(GameStage.Response);

        switch (feedback)
        {
            case "Good":
                currentDialogue = mainDialogue.ResponseGood;
                break;
            case "Average":
                currentDialogue = mainDialogue.ResponseAverage; 
                break;
            case "Bad":
                currentDialogue = mainDialogue.ResponseBad;
                break;
        }
    }

    public void ProgressStage()
    {
        GameStage stage = StageManagerRef.GetCurrentGameStage();
        List<string> nextDialogue = new();

        switch (stage) 
        {
            case GameStage.EnterStore:
                StageManagerRef.SetCurrentGameStage(GameStage.Greeting);
                currentDialogue = mainDialogue.Greeting;
                break;
            case GameStage.Greeting:
                StageManagerRef.SetCurrentGameStage(GameStage.Request);
                currentDialogue = mainDialogue.Request;
                break;
            case GameStage.Request:
                // enable flashing arrows here
            case GameStage.Response:
                StageManagerRef.SetCurrentGameStage(GameStage.LeaveStore);
                break;
        }
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
