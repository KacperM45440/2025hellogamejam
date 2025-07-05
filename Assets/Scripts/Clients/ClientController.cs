using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public ClientData ClientDataRef;
    public ClientScript ClientRef;
    public int ClientAmount;
    public int CurrentClientInt;

    [HideInInspector] public List<string> ClientNames;
    [HideInInspector] public List<Sprite> ClientBodies;
    [HideInInspector] public List<Sprite> ClientFaces;
    [HideInInspector] public List<Mesh> ClientHeads;
    [HideInInspector] public List<string> ClientSpeeches;

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
        ClientNames = ClientDataRef.CreateClientNames();
        ClientBodies = ClientDataRef.CreateClientBodies();
        ClientFaces = ClientDataRef.CreateClientFaces();
        ClientHeads = ClientDataRef.CreateClientHeads();
        ClientSpeeches = ClientDataRef.CreateClientSpeeches();
    }

    public Client GetNextClient(int index)
    {
        Client nextClient = new Client();
        
        nextClient.ClientName = ClientNames[index];
        nextClient.ClientBody = ClientBodies[index];
        nextClient.ClientFace = ClientFaces[index];
        nextClient.ClientHead = ClientHeads[index];
        nextClient.ClientSpeech = ClientSpeeches[index];

        return nextClient;
    }

    public void CreateNextClient()
    {
        Client currentClient = GetNextClient(CurrentClientInt);
        ClientRef.ClientHead.mesh = currentClient.ClientHead;
        ClientRef.ClientBody.sprite = currentClient.ClientBody;
        CurrentClientInt++;
    }

    public class Client
    {
        public int ClientId;
        public string ClientName;
        public Sprite ClientBody;
        public Sprite ClientFace;
        public Mesh ClientHead;
        public string ClientSpeech;
    }
}
