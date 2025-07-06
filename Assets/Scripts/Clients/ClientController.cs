using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public MoneyController moneyControllerRef;
    public DialogueController dialogueControllerRef;
    public ClientData ClientDataRef;
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

    private int currentClientSatisfaction = 0;

    public void Start()
    {
        InitializeClients();
    }

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateNextClient();
        }
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
        if (ClientRef.transform.childCount > 2)
        {
            Destroy(ClientRef.transform.GetChild(2).gameObject);
        }

        CurrentClient = GetNextClient(CurrentClientInt);

        GameObject newHead = Instantiate(CurrentClient.ClientHead, ClientRef.transform);
        newHead.transform.Rotate(new Vector3(0, 0, 90));
        newHead.transform.localPosition = new Vector3(newHead.transform.localPosition.x, newHead.transform.localPosition.y + CurrentClient.ClientHeadOffset.y, newHead.transform.localPosition.z);

        ClientRef.ClientHead = newHead;

        ClientRef.ClientBody.sprite = CurrentClient.ClientBody;

        CurrentClientInt++;

        //W³¹cz animacjê klienta podchodz¹cego do lady
        ClientRef.GetComponent<Animator>().SetTrigger("EnterShop");
        StartCoroutine(WaitForClientArrival());
    }

    private IEnumerator WaitForClientArrival()
    {
        yield return new WaitForSeconds(5f); // CZAS TRWANIA ANIMACJI WEJŒCIA KLIENTA
        ClientArrived();
    }

    //Metoda wywo³ana z animacji
    public void ClientArrived()
    {
        dialogueControllerRef.ProgressStage();
        dialogueControllerRef.ProgressDialogue();
    }

    public void ClientReceiveGun()
    {
        ClientRef.GetComponent<Animator>().SetTrigger("InspectGun");
        StartCoroutine(WaitForGunInspection());
    }

    private IEnumerator WaitForGunInspection()
    {
        yield return new WaitForSeconds(5f); // CZAS TRWANIA ANIMACJI WEJŒCIA KLIENTA
        //ClientReviewGun();
    }

    public void ClientReviewGun(List<ItemCharacteristics> itemCharacteristics)
    {
        //DO NAPRAWIENIA
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
