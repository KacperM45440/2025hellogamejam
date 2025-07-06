using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameFlowController FlowControllerRef;

    public GameObject blackScreen;
    public TextMeshProUGUI dayCount;
    public GameObject continuePrompt;

    private bool clickToContinue = false;

    private string controllerName = "UIController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

    private void Update()
    {
        if(clickToContinue && Input.GetMouseButtonDown(0))
        {
            clickToContinue = false;
            blackScreen.SetActive(false);
            dayCount.gameObject.SetActive(false);
            continuePrompt.SetActive(false);
            FlowControllerRef.FinishRequirement(ControllerName);
        }
    }

    public void FinishDay()
    {
        StartCoroutine(FinishDayTiming());
    }

    private IEnumerator FinishDayTiming()
    {
        yield return new WaitForSeconds(2f);
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        dayCount.text = "Day " + FlowControllerRef.currentDay.ToString();
        dayCount.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        dayCount.text = "Day " + (FlowControllerRef.currentDay + 1).ToString();
        yield return new WaitForSeconds(2f);
        continuePrompt.SetActive(true);
        clickToContinue = true;
    }
}
