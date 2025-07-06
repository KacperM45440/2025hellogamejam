using UnityEngine;

public class BubbleController : MonoBehaviour
{
    private Vector3 StartPosition;
    private RectTransform rectRef;

    private void Start()
    {
        rectRef = GetComponent<RectTransform>();
        StartPosition = rectRef.localPosition;
    }
    public void NextText(int index, string clientName, string clientSpeech)
    {
        GameObject nextEntry = transform.GetChild(index).gameObject;
        nextEntry.GetComponent<SpeechScript>().ClientNameTMP.text = clientName;
        nextEntry.GetComponent<SpeechScript>().ClientSpeechTMP.text = clientSpeech;
        nextEntry.SetActive(true);
        MoveTextUp(100);
    }

    public void MoveTextUp(int distance)
    {
        rectRef.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + distance, transform.localPosition.z); // placeholder, should be animated
    }

    public void ClearText()
    {
        foreach (Transform bubble in transform)
        {
            bubble.gameObject.SetActive(false);
        }

        rectRef.localPosition = StartPosition;
    }
}
