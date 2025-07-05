using System.Collections.Generic;
using UnityEngine;

public class DialogueData : MonoBehaviour
{
    public List<string> Client1Greeting;
    public List<string> Client1Request;
    public List<string> Client1ResponseGood;
    public List<string> Client1ResponseAverage;
    public List<string> Client1ResponseBad;

    [Space]

    public List<string> Client2Greeting;
    public List<string> Client2Request;
    public List<string> Client2ResponseGood;
    public List<string> Client2ResponseAverage;
    public List<string> Client2ResponseBad;

    public List<List<string>> CreateDialogueGreeting()
    {
        List<List<string>> clientGreets = new List<List<string>>
        {
            Client1Greeting,
            Client2Greeting,
        };

        return clientGreets;
    }

    public List<List<string>> CreateDialogueRequest()
    {
        List<List<string>> clientRequests = new List<List<string>>
        {
            Client1Request,
            Client2Request,
        };

        return clientRequests;
    }

    public List<List<string>> CreateResponseGood()
    {
        List<List<string>> clientResponsesGood = new List<List<string>>
        {
            Client1ResponseGood,
            Client2ResponseGood,
        };

        return clientResponsesGood;
    }

    public List<List<string>> CreateResponseAverage()
    {
        List<List<string>> clientResponsesAverage = new List<List<string>>
        {
            Client1ResponseAverage,
            Client2ResponseAverage,
        };

        return clientResponsesAverage;
    }

    public List<List<string>> CreateResponseBad()
    {
        List<List<string>> clientResponsesBad = new List<List<string>>
        {
            Client1ResponseBad,
            Client2ResponseBad,
        };

        return clientResponsesBad;
    }
}
