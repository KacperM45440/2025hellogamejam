using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    public ClientData ClientDataRef;
    public int ClientAmount;
    public int CurrentClientInt;
    public GameObject CurrentClientGO;

    [HideInInspector] public List<string> ClientNames;
    [HideInInspector] public List<Sprite> ClientBodies;
    [HideInInspector] public List<Sprite> ClientFaces;
    [HideInInspector] public List<Mesh> ClientHeads;
    [HideInInspector] public List<string> ClientSpeeches;

    public void Start()
    {
        InitializeClients();
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

    public void CreateClient()
    {
        Client currentClient = GetNextClient(CurrentClientInt);
        CurrentClientGO.GetComponent<MeshFilter>().mesh = currentClient.ClientHead;
        CurrentClientInt++;
    }

    public class Client
    {
        public string ClientName;
        public Sprite ClientBody;
        public Sprite ClientFace;
        public Mesh ClientHead;
        public string ClientSpeech;
    }
}
