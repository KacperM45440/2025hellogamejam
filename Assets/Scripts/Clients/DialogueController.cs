using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{

    public class Dialogue
    {
        public List<string> Greeting;
        public List<string> Request;
        public List<string> ResponseGood;
        public List<string> ResponseAverage;
        public List<string> ResponseBad;
    }
}
