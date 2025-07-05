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

    [Space]

    public List<string> Client3Greeting;
    public List<string> Client3Request;
    public List<string> Client3ResponseGood;
    public List<string> Client3ResponseAverage;
    public List<string> Client3ResponseBad;

    [Space]

    public List<string> Client4Greeting;
    public List<string> Client4Request;
    public List<string> Client4ResponseGood;
    public List<string> Client4ResponseAverage;
    public List<string> Client4ResponseBad;

    [Space]

    public List<string> Client5Greeting;
    public List<string> Client5Request;
    public List<string> Client5ResponseGood;
    public List<string> Client5ResponseAverage;
    public List<string> Client5ResponseBad;

    [Space]

    public List<string> Client6Greeting;
    public List<string> Client6Request;
    public List<string> Client6ResponseGood;
    public List<string> Client6ResponseAverage;
    public List<string> Client6ResponseBad;

    [Space]

    public List<string> Client7Greeting;
    public List<string> Client7Request;
    public List<string> Client7ResponseGood;
    public List<string> Client7ResponseAverage;
    public List<string> Client7ResponseBad;

    [Space]

    public List<string> Client8Greeting;
    public List<string> Client8Request;
    public List<string> Client8ResponseGood;
    public List<string> Client8ResponseAverage;
    public List<string> Client8ResponseBad;

    [Space]

    public List<string> Client9Greeting;
    public List<string> Client9Request;
    public List<string> Client9ResponseGood;
    public List<string> Client9ResponseAverage;
    public List<string> Client9ResponseBad;

    public List<List<string>> CreateDialogueGreeting()
    {
        List<List<string>> clientGreets = new List<List<string>>
        {
            Client1Greeting,
            Client2Greeting,
            Client3Greeting,
            Client4Greeting,
            Client5Greeting,
            Client6Greeting,
            Client7Greeting,
            Client8Greeting,
            Client9Greeting,
        };

        return clientGreets;
    }

    public List<List<string>> CreateDialogueRequest()
    {
        List<List<string>> clientRequests = new List<List<string>>
        {
            Client1Request,
            Client2Request,
            Client3Request,
            Client4Request,
            Client5Request,
            Client6Request,
            Client7Request,
            Client8Request,
            Client9Request,
        };

        return clientRequests;
    }

    public List<List<string>> CreateResponseGood()
    {
        List<List<string>> clientResponsesGood = new List<List<string>>
        {
            Client1ResponseGood,
            Client2ResponseGood,
            Client3ResponseGood,
            Client4ResponseGood,
            Client5ResponseGood,
            Client6ResponseGood,
            Client7ResponseGood,
            Client8ResponseGood,
            Client9ResponseGood,
        };

        return clientResponsesGood;
    }

    public List<List<string>> CreateResponseAverage()
    {
        List<List<string>> clientResponsesAverage = new List<List<string>>
        {
            Client1ResponseAverage,
            Client2ResponseAverage,
            Client3ResponseAverage,
            Client4ResponseAverage,
            Client5ResponseAverage,
            Client6ResponseAverage,
            Client7ResponseAverage,
            Client8ResponseAverage,
            Client9ResponseAverage,
        };

        return clientResponsesAverage;
    }

    public List<List<string>> CreateResponseBad()
    {
        List<List<string>> clientResponsesBad = new List<List<string>>
        {
            Client1ResponseBad,
            Client2ResponseBad,
            Client3ResponseBad,
            Client4ResponseBad,
            Client5ResponseBad,
            Client6ResponseBad,
            Client7ResponseBad,
            Client8ResponseBad,
            Client9ResponseBad,
        };

        return clientResponsesBad;
    }
}
