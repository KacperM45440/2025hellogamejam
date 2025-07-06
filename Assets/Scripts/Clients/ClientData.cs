using System.Collections.Generic;
using UnityEngine;

public class ClientData : MonoBehaviour
{
    public int Client1ID = 1;
    public string Client1Name;
    public Sprite Client1Body;
    public Sprite Client1Face;
    public Mesh Client1Head;
    public string Client1Speech;

    [Space]

    public int Client2ID = 2;
    public string Client2Name;
    public Sprite Client2Body;
    public Sprite Client2Face;
    public Mesh Client2Head;
    public string Client2Speech;

    [Space]
    
    public int Client3ID = 3;
    public string Client3Name;
    public Sprite Client3Body;
    public Sprite Client3Face;
    public Mesh Client3Head;
    public string Client3Speech;

    [Space]

    public int Client4ID = 4;
    public string Client4Name;
    public Sprite Client4Body;
    public Sprite Client4Face;
    public Mesh Client4Head;
    public string Client4Speech;

    [Space]

    public int Client5ID = 5;
    public string Client5Name;
    public Sprite Client5Body;
    public Sprite Client5Face;
    public Mesh Client5Head;
    public string Client5Speech;

    [Space]

    public int Client6ID = 6;
    public string Client6Name;
    public Sprite Client6Body;
    public Sprite Client6Face;
    public Mesh Client6Head;
    public string Client6Speech;

    [Space]

    public int Client7ID = 7;
    public string Client7Name;
    public Sprite Client7Body;
    public Sprite Client7Face;
    public Mesh Client7Head;
    public string Client7Speech;

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

    public List<Sprite> CreateClientFaces()
    {
        List<Sprite> clientFaces = new List<Sprite>
        {
            Client1Face,
            Client2Face,
            Client3Face,
            Client4Face,
            Client5Face,
            Client6Face,
            Client7Face
        };

        return clientFaces;
    }

    public List<Mesh> CreateClientHeads()
    {
        List<Mesh> clientHeads = new List<Mesh>
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
}
