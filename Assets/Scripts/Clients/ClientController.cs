using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
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

    public void Start()
    {
        InitializeClients();
        todaysClients.Add(1);
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

    public void CreateNextClient()
    {
        if (ClientRef.transform.childCount > 3)
        {
            Destroy(ClientRef.transform.GetChild(3).gameObject);
        }

        CurrentClient = GetNextClient(CurrentClientInt);
        BubbleRef.ClearText();
        dialogueControllerRef.ChangeMainDialogue(CurrentClientInt);
        StageManagerRef.SetCurrentGameStage(StageManager.GameStage.EnterStore);
        

        GameObject newHead = Instantiate(CurrentClient.ClientHead, ClientRef.transform);
        newHead.transform.Rotate(new Vector3(0, 0, 90));
        newHead.transform.localPosition = new Vector3(newHead.transform.localPosition.x, newHead.transform.localPosition.y + CurrentClient.ClientHeadOffset.y, newHead.transform.localPosition.z);

        ClientRef.ClientHead = newHead;

        ClientRef.ClientBody.sprite = CurrentClient.ClientBody;
        ClientRef.Outline.sprite = CurrentClient.ClientBody;

        CurrentClientInt++;


        //W��cz animacj� klienta podchodz�cego do lady
        ClientRef.GetComponent<Animator>().SetTrigger("EnterShop");
        StartCoroutine(WaitForClientArrival());
    }

    private IEnumerator WaitForClientArrival()
    {
        yield return new WaitForSeconds(5f); // CZAS TRWANIA ANIMACJI WEJ�CIA KLIENTA
        ClientArrived();
    }

    public void ClientArrived()
    {
        dialogueControllerRef.ProgressStage();
        dialogueControllerRef.ProgressDialogue();
    }

    public void ClientReceiveGun(Item gun)
    {
        ClientRef.GetComponent<Animator>().SetTrigger("InspectGun");
        StartCoroutine(WaitForGunInspection(gun));
    }

    private IEnumerator WaitForGunInspection(Item gun)
    {
        yield return new WaitForSeconds(3f); // CZAS TRWANIA ANIMACJI WEJ�CIA KLIENTA
        List<ItemCharacteristics> itemCharacteristicsList = new List<ItemCharacteristics>();
        itemCharacteristicsList.AddRange(gun.characteristics);
        for (int i = 0; i < gun.itemAnchors.Length; i++)
        {
            itemCharacteristicsList.AddRange(gun.itemAnchors[i].anchor.addedItem.characteristics);
        }

        ClientReviewGun(itemCharacteristicsList);
    }

    public void ClientReviewGun(List<ItemCharacteristics> itemCharacteristics)
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

        int payment = DefaultClientPayment;
        if (currentClientSatisfaction < 0)
        {
            payment = 50;
        }
        else
        {
            payment += (50 * currentClientSatisfaction);
        }

        Debug.Log("Final payment is: " + payment);
        moneyControllerRef.gainMoney(payment);
        ClientRef.GetComponent<Animator>().speed = -1f;
        ClientRef.GetComponent<Animator>().SetTrigger("EnterShop");
        StartCoroutine(WaitAfterGivingItem());
    }

    private IEnumerator WaitAfterGivingItem()
    {
        yield return new WaitForSeconds(3f);

        //SPRAWDZ CZY JEST JESZCZE DZISIAJ KLIENT, JAK NIE, POKAZ TABLETA
        CreateNextClient();
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
