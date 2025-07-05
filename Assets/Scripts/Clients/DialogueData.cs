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
}
