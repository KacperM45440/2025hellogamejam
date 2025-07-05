using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public DialogueController dialogueControllerRef;
    public ClientData ClientDataRef;
    public ClientScript ClientRef;
    public int ClientAmount;
    public int CurrentClientInt;

    [HideInInspector] public List<int> ClientIds;
    [HideInInspector] public List<Sprite> ClientBodies;
    [HideInInspector] public List<Sprite> ClientFaces;
    [HideInInspector] public List<Mesh> ClientHeads;

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
        ClientBodies = ClientDataRef.CreateClientBodies();
        ClientFaces = ClientDataRef.CreateClientFaces();
        ClientHeads = ClientDataRef.CreateClientHeads();
    }

    public Client GetNextClient(int index)
    {
        Client nextClient = new Client();
        
        nextClient.ClientId = ClientIds[index];
        nextClient.ClientBody = ClientBodies[index];
        nextClient.ClientFace = ClientFaces[index];
        nextClient.ClientHead = ClientHeads[index];

        return nextClient;
    }

    public void CreateNextClient()
    {
        Client currentClient = GetNextClient(CurrentClientInt);

        ClientRef.ClientHead.mesh = currentClient.ClientHead;
        ClientRef.ClientBody.sprite = currentClient.ClientBody;
        ClientRef.ClientFace.sprite = currentClient.ClientFace;

        CurrentClientInt++;

        //W³¹cz animacjê klienta podchodz¹cego do lady
        //ClientRef.GetComponent<Animator>().SetTrigger("WalkToCounter");
    }

    //Metoda wywo³ana z animacji
    public void ClientArrived()
    {
        dialogueControllerRef.ProgressStage();
        dialogueControllerRef.ProgressDialogue();
    }

    public class Client
    {
        public int ClientId;
        public Sprite ClientBody;
        public Sprite ClientFace;
        public Mesh ClientHead;
    }
}
