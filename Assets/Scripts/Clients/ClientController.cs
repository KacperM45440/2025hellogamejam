using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ClientController : MonoBehaviour
{
    public enum ClientMood
    {
        Bad,
        Neutral,
        Good
    }

    public GameFlowController GameFlowControllerRef;
    public MoneyController moneyControllerRef;
    public DialogueController dialogueControllerRef;
    public StageManager StageManagerRef;
    public ClientData ClientDataRef;
    public BubbleController BubbleRef;
    public ClientScript ClientRef;
    public Client CurrentClient;
    public int ClientAmount;
    public int CurrentClientInt;
    public int DefaultClientPayment = 100;

    [HideInInspector] public List<int> ClientIds;
    [HideInInspector] public List<string> ClientNames;
    [HideInInspector] public List<Sprite> ClientBodies;
    [HideInInspector] public List<GameObject> ClientHeads;
    [HideInInspector] public List<Vector3> ClientOffsets;
    [HideInInspector] public List<List<ItemCharacteristics>> ClientPrefs;
    [HideInInspector] public List<List<ItemCharacteristics>> ClientHates;

    public List<int> todaysClients = new List<int>();

    private int currentClientSatisfaction = 0;
    private ClientMood currentClientMood;
    private bool canReveiveGun = false;

    private string controllerName = "ClientController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

    public void Start()
    {
        InitializeClients();
        todaysClients.Add(0);
    }

    public void InitializeClients()
    {
        ClientIds = ClientDataRef.CreateClientIDs();
        ClientNames = ClientDataRef.CreateClientNames();
        ClientBodies = ClientDataRef.CreateClientBodies();
        ClientHeads = ClientDataRef.CreateClientHeads();
        ClientOffsets = ClientDataRef.CreateClientOffsets();
        ClientPrefs = ClientDataRef.CreatePrefferedCharacteristics();
        ClientHates = ClientDataRef.CreateHatedCharacteristics();
    }

    public Client GetNextClient(int index)
    {
        currentClientSatisfaction = 0;

        Client nextClient = new Client();
        
        nextClient.ClientId = ClientIds[index];
        nextClient.ClientName = ClientNames[index];
        nextClient.ClientBody = ClientBodies[index];
        nextClient.ClientHead = ClientHeads[index];
        nextClient.ClientHeadOffset = ClientOffsets[index];
        nextClient.PrefferedItemCharacteristics = ClientPrefs[index];
        nextClient.HatedItemCharacteristics = ClientHates[index];

        return nextClient;
    }

    public int GetTodaysClientsCount()
    {
        return todaysClients.Count;
    }

    public void CreateNextClient()
    {
        if (ClientRef.transform.childCount > 3)
        {
            Destroy(ClientRef.transform.GetChild(3).gameObject);
        }
        CurrentClientInt = todaysClients[0];
        CurrentClient = GetNextClient(CurrentClientInt);
        BubbleRef.ClearText();
        dialogueControllerRef.ChangeMainDialogue(CurrentClientInt);
        //StageManagerRef.SetCurrentGameStage(StageManager.GameStage.ClientEnterStore);

        GameObject newHead = Instantiate(CurrentClient.ClientHead, ClientRef.transform);
        newHead.transform.Rotate(new Vector3(0, 0, 90));
        newHead.transform.localPosition = new Vector3(newHead.transform.localPosition.x, newHead.transform.localPosition.y + CurrentClient.ClientHeadOffset.y, newHead.transform.localPosition.z);

        ClientRef.ClientHead = newHead;

        ClientRef.ClientBody.sprite = CurrentClient.ClientBody;
        ClientRef.Outline.sprite = CurrentClient.ClientBody;

        ClientRef.GetComponent<Animator>().SetTrigger("EnterShop");
        StartCoroutine(WaitForClientArrival());
    }

    private IEnumerator WaitForClientArrival()
    {
        yield return new WaitForSeconds(5f); // CZAS TRWANIA ANIMACJI WEJ�CIA KLIENTA
        GameFlowControllerRef.FinishRequirement(controllerName); // Informujemy GameFlowController, że klient dotarł
        //ClientArrived();
    }

    public void ClientArrived()
    {
        //TO zrobi Game Flow Controller
        /*
        dialogueControllerRef.ProgressStage();
        dialogueControllerRef.ProgressDialogue();
        */
    }

    public void SetClientCanReceiveGun(bool canReceive)
    {
        canReveiveGun = canReceive;
    }

    public bool GetClientCanReceiveGun()
    {
        return canReveiveGun;
    }

    public void ClientReceiveGun(Item gun)
    {
        if(!canReveiveGun)
        {
            return;
        }
        canReveiveGun = false;

        List<ItemCharacteristics> itemCharacteristicsList = new List<ItemCharacteristics>();
        itemCharacteristicsList.AddRange(gun.characteristics);
        for (int i = 0; i < gun.itemAnchors.Length; i++)
        {
            if (gun.itemAnchors[i].anchor.addedItem != null)
            {
                itemCharacteristicsList.AddRange(gun.itemAnchors[i].anchor.addedItem.characteristics);
            }
        }
        ClientCalculateSatisfaction(itemCharacteristicsList);
    }

    public void ClientCalculateSatisfaction(List<ItemCharacteristics> itemCharacteristics)
    {
        foreach (ItemCharacteristics characteristic in itemCharacteristics)
        {
            if (CurrentClient.PrefferedItemCharacteristics.Contains(characteristic))
            {
                currentClientSatisfaction += 3;
            }
            else if (CurrentClient.HatedItemCharacteristics.Contains(characteristic))
            {
                currentClientSatisfaction -= 2;
            }
            else
            {
                currentClientSatisfaction++;
            }
        }

        switch (currentClientSatisfaction)
        {
            case <= 0:
                currentClientMood = ClientMood.Bad;
                break;
            case <= 5:
                currentClientMood = ClientMood.Neutral;
                break;
            default:
                currentClientMood = ClientMood.Good;
                break;
        }

        ClientRef.GetComponent<Animator>().SetTrigger("InspectGun");
        StartCoroutine(WaitForGunInspection());
    }

    private IEnumerator WaitForGunInspection()
    {
        yield return new WaitForSeconds(3f);
        GameFlowControllerRef.FinishRequirement(controllerName);

        //dialogueControllerRef.WeaponFeedback(currentClientMood);

        //PIENIĄDZE POWINIEN PRZEKAZAĆ PO ODDANIU FEEDBACKU
        //Debug.Log("Final payment is: " + payment);
        //moneyControllerRef.gainMoney(payment);
    }

    public ClientMood GetCurrentClientMood()
    {
        return currentClientMood;
    }

    private int CalculatePayment()
    {
        int payment = DefaultClientPayment;
        if (currentClientSatisfaction <= 0)
        {
            payment = 50;
        }
        else
        {
            payment += (50 * currentClientSatisfaction);
        }
        return payment;
    }

    public void ClientPaysThenLeaves()
    {
        //ClientRef.GetComponent<Animator>().SetTrigger("Pay");
        StartCoroutine(WaitAfterPaying());
    }

    private IEnumerator WaitAfterPaying()
    {
        yield return new WaitForSeconds(2f);
        int payment = CalculatePayment();
        moneyControllerRef.gainMoney(payment);
        yield return new WaitForSeconds(2f);
        ClientRef.GetComponent<Animator>().SetTrigger("ExitShop");
        StartCoroutine(WaitAfterLeavingShop());
    }

    private IEnumerator WaitAfterLeavingShop()
    {
        yield return new WaitForSeconds(3f);

        //Tylko aby wizualnie klient zniknął póki nie ma animacji wychodzenia
        ClientRef.transform.position = new Vector3(ClientRef.transform.position.x, ClientRef.transform.position.y - 1f, ClientRef.transform.position.z);

        todaysClients.RemoveAt(0);
        GameFlowControllerRef.FinishRequirement(controllerName);
    }

    public void ClientGaveItem()
    {
        ClientRef.transform.position = new Vector3(ClientRef.transform.position.x, ClientRef.transform.position.y - 1f, ClientRef.transform.position.z);
        StartCoroutine(WaitAfterGivingItem());
    }

    private IEnumerator WaitAfterGivingItem()
    {
        yield return new WaitForSeconds(3f);
        //SPRAWDZ CZY JEST JESZCZE DZISIAJ KLIENT, JAK NIE, POKAZ TABLETA

        todaysClients.Remove(CurrentClient.ClientId);
        if(todaysClients.Count > 0)
        {
            CreateNextClient();
        }
        else
        {
            //wyciagnij tableta
            //StageManagerRef.SetCurrentGameStage(StageManager.GameStage.Tablet);
        }
    }

    public class Client
    {
        public int ClientId;
        public string ClientName;
        public Sprite ClientBody;
        public GameObject ClientHead;
        public Vector3 ClientHeadOffset;
        public List<ItemCharacteristics> PrefferedItemCharacteristics;
        public List<ItemCharacteristics> HatedItemCharacteristics;
    }
}
