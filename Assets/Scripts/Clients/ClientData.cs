using System.Collections.Generic;
using UnityEngine;

public class ClientData : MonoBehaviour
{
    public int Client1ID = 1;
    public string Client1Name;
    public Sprite Client1Body;
    public GameObject Client1Head;
    public Vector3 Client1HeadOffset;
    public string Client1Speech;
    public List<ItemCharacteristics> Client1PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client1HatedItemCharacteristics;

    [Space]

    public int Client2ID = 2;
    public string Client2Name;
    public Sprite Client2Body;
    public GameObject Client2Head;
    public Vector3 Client2HeadOffset;
    public string Client2Speech;
    public List<ItemCharacteristics> Client2PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client2HatedItemCharacteristics;

    [Space]
    
    public int Client3ID = 3;
    public string Client3Name;
    public Sprite Client3Body;
    public GameObject Client3Head;
    public Vector3 Client3HeadOffset;
    public string Client3Speech;
    public List<ItemCharacteristics> Client3PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client3HatedItemCharacteristics;

    [Space]

    public int Client4ID = 4;
    public string Client4Name;
    public Sprite Client4Body;
    public GameObject Client4Head;
    public Vector3 Client4HeadOffset;
    public string Client4Speech;
    public List<ItemCharacteristics> Client4PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client4HatedItemCharacteristics;

    [Space]

    public int Client5ID = 5;
    public string Client5Name;
    public Sprite Client5Body;
    public GameObject Client5Head;
    public Vector3 Client5HeadOffset;
    public string Client5Speech;
    public List<ItemCharacteristics> Client5PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client5HatedItemCharacteristics;

    [Space]

    public int Client6ID = 6;
    public string Client6Name;
    public Sprite Client6Body;
    public GameObject Client6Head;
    public Vector3 Client6HeadOffset;
    public string Client6Speech;
    public List<ItemCharacteristics> Client6PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client6HatedItemCharacteristics;

    [Space]

    public int Client7ID = 7;
    public string Client7Name;
    public Sprite Client7Body;
    public GameObject Client7Head;
    public Vector3 Client7HeadOffset;
    public string Client7Speech;
    public List<ItemCharacteristics> Client7PrefferedItemCharacteristics;
    public List<ItemCharacteristics> Client7HatedItemCharacteristics;

    public List<int> CreateClientIDs()
    {
        List<int> clientIDs = new List<int>
        {
            Client1ID,
            Client2ID,
            Client3ID,
            Client4ID,
            Client5ID,
            Client6ID,
            Client7ID
        };

        return clientIDs;
    }

    public List<string> CreateClientNames()
    {
        List<string> clientNames = new List<string>
        {
            Client1Name,
            Client2Name, 
            Client3Name, 
            Client4Name,
            Client5Name, 
            Client6Name,
            Client7Name
        };

        return clientNames;
    }

    public List<Sprite> CreateClientBodies()
    {
        List<Sprite> clientBodies = new List<Sprite>
        {
            Client1Body,
            Client2Body,
            Client3Body,
            Client4Body,
            Client5Body,
            Client6Body,
            Client7Body
        };

        return clientBodies;
    }

    public List<GameObject> CreateClientHeads()
    {
        List<GameObject> clientHeads = new List<GameObject>
        {
            Client1Head,
            Client2Head,
            Client3Head,
            Client4Head,
            Client5Head,
            Client6Head,
            Client7Head
        };

        return clientHeads;
    }

    public List<Vector3> CreateClientOffsets()
    {
        List<Vector3> clientOffsets = new List<Vector3>
        {
            Client1HeadOffset,
            Client2HeadOffset,
            Client3HeadOffset,
            Client4HeadOffset,
            Client5HeadOffset,
            Client6HeadOffset,
            Client7HeadOffset
        };

        return clientOffsets;
    }

    public List<string> CreateClientSpeeches()
    {
        List<string> clientSpeech = new List<string>
        {
            Client1Speech,
            Client2Speech,
            Client3Speech,
            Client4Speech,
            Client5Speech,
            Client6Speech,
            Client7Speech
        };

        return clientSpeech;
    }

    public List<List<ItemCharacteristics>> CreatePrefferedCharacteristics()
    {
        List<List<ItemCharacteristics>> clientPrefs = new List<List<ItemCharacteristics>>
        {
            Client1PrefferedItemCharacteristics,
            Client2PrefferedItemCharacteristics,
            Client3PrefferedItemCharacteristics,
            Client4PrefferedItemCharacteristics,
            Client5PrefferedItemCharacteristics,
            Client6PrefferedItemCharacteristics,
            Client7PrefferedItemCharacteristics
        };

        return clientPrefs;
    }

    public List<List<ItemCharacteristics>> CreateHatedCharacteristics()
    {
        List<List<ItemCharacteristics>> clientHates = new List<List<ItemCharacteristics>>
        {
            Client1HatedItemCharacteristics,
            Client2HatedItemCharacteristics,
            Client3HatedItemCharacteristics,
            Client4HatedItemCharacteristics,
            Client5HatedItemCharacteristics,
            Client6HatedItemCharacteristics,
            Client7HatedItemCharacteristics
        };

        return clientHates;
    }
}
