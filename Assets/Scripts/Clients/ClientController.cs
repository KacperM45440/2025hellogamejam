using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
    public InventoryController inventoryControllerRef;
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
    private int currentGunValue = 0;
    private ClientMood currentClientMood;
    private bool canReveiveGun = false;
    [SerializeField] private Cardboarder cardboarder;

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

    public void AddNextDaysClient(int clientId)
    {
        if (!todaysClients.Contains(clientId))
        {
            todaysClients.Add(clientId);
            Debug.Log("Added client with ID: " + clientId + " to today's clients.");
        }
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
        
        cardboarder.Initialize(ClientRef.ClientBody);

    }

    private IEnumerator WaitForClientArrival()
    {
        yield return new WaitForSeconds(2.5f);
        Door.Instance.OpenDoor();
        yield return new WaitForSeconds(2.5f); // CZAS TRWANIA ANIMACJI WEJ�CIA KLIENTA
       
        GameFlowControllerRef.FinishRequirement(controllerName); // Informujemy GameFlowController, że klient dotarł
        yield return new WaitForSeconds(1f);
        Door.Instance.CloseDoor();
        //ClientArrived();
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
        
        gun.transform.DOKill();
        gun.transform.parent = ClientRef.gunSocket;
        gun.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.InOutBack);
        gun.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.25f).SetEase(Ease.InOutBack);

        List<ItemCharacteristics> itemCharacteristicsList = new List<ItemCharacteristics>();
        itemCharacteristicsList.AddRange(gun.characteristics);
        inventoryControllerRef.RemoveFromInventoryByName(gun.name);

        currentGunValue = 0;
        int items = 0;
        for (int i = 0; i < gun.itemAnchors.Length; i++)
        {
            Item item = gun.itemAnchors[i].anchor.addedItem;
            if (item == null)
            {
                continue;
            }
            items++;
            currentGunValue += item.price;
            itemCharacteristicsList.AddRange(item.characteristics);
            inventoryControllerRef.RemoveFromInventoryByName(item.name);
        }
        inventoryControllerRef.CheckFrameExistenceAndSpawnIfNeeded();
        ClientCalculateSatisfaction(itemCharacteristicsList, items);
    }

    public void ClientCalculateSatisfaction(List<ItemCharacteristics> itemCharacteristics, int itemCount)
    {
        if(itemCount <= 2)
        {
            currentClientSatisfaction -= 1;
        }
        else
        {
            foreach (ItemCharacteristics characteristic in itemCharacteristics)
            {
                if (CurrentClient.PrefferedItemCharacteristics.Contains(characteristic))
                {
                    currentClientSatisfaction += 3;
                }
                else if (CurrentClient.HatedItemCharacteristics.Contains(characteristic))
                {
                    currentClientSatisfaction -= 3;
                }
                else
                {
                    currentClientSatisfaction++;
                }
            }
        }

        switch (currentClientSatisfaction)
        {
            case <= 2:
                currentClientMood = ClientMood.Bad;
                break;
            case <= 6:
                currentClientMood = ClientMood.Neutral;
                break;
            default:
                currentClientMood = ClientMood.Good;
                break;
        }
        Debug.Log("Current client mood: " + currentClientMood + " with satisfaction: " + currentClientSatisfaction);

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
        switch (currentClientMood)
        {
            case ClientMood.Bad:
                payment = Random.Range(30, 70);
                break;
            case ClientMood.Neutral:
                payment = (int)(currentGunValue * 1.3f);
                break;
            case ClientMood.Good:
                payment = (int)(currentGunValue * 1.3f) + currentClientSatisfaction * 10;
                break;
        }
        return payment;
    }

    public void ClientPaysThenLeaves()
    {
        ClientRef.GetComponent<Animator>().SetTrigger("Pay");
        StartCoroutine(WaitAfterPaying());
    }

    private IEnumerator WaitAfterPaying()
    {
        yield return new WaitForSeconds(2f);
        int payment = CalculatePayment();
        moneyControllerRef.gainMoney(payment);
        Door.Instance.OpenDoor();
        yield return new WaitForSeconds(2f);
        ClientRef.GetComponent<Animator>().SetTrigger("ExitShop");
        StartCoroutine(WaitAfterLeavingShop());
    }

    private IEnumerator WaitAfterLeavingShop()
    {
        yield return new WaitForSeconds(3f);
        Door.Instance.CloseDoor();
        //Tylko aby wizualnie klient zniknął póki nie ma animacji wychodzenia
        ClientRef.transform.position = new Vector3(ClientRef.transform.position.x, ClientRef.transform.position.y - 1f, ClientRef.transform.position.z);

        todaysClients.RemoveAt(0);
        GameFlowControllerRef.FinishRequirement(controllerName);
        
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
