using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameFlowController FlowControllerRef;

    public GameObject blackScreen;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI dayCountOld;
    public TextMeshProUGUI dayCountNew;
    public TextMeshProUGUI lastDayText;
    public GameObject continuePrompt;

    private string controllerName = "UIController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

    public void FinishDay()
    {
        dayCountOld.text = FlowControllerRef.currentDay.ToString();
        dayCountNew.text = (FlowControllerRef.currentDay + 1).ToString();

        blackScreen.SetActive(true);
        Image blackScreenBackground = blackScreen.GetComponent<Image>();
        blackScreenBackground.DOFade(1f, 2f).SetDelay(2f).OnComplete(() =>
        {
            dayText.gameObject.SetActive(true);
            dayText.DOFade(1f, 0.25f).SetDelay(1f).OnComplete(() =>
            {
                dayCountOld.gameObject.SetActive(true);
                dayCountOld.DOFade(1f, 0.25f).From(0).SetDelay(1f).OnComplete(() =>
                {
                    dayCountOld.DOFade(0f, 0.5f).SetDelay(1.25f);
                    dayCountOld.transform.DOLocalMove(new Vector3(0f, -300f, 0), 0.5f).SetDelay(1.25f);
                    dayCountNew.gameObject.SetActive(true);
                    dayCountNew.transform.DOLocalMove(new Vector3(0f, 0, 0), 0.5f).SetDelay(1f);
                    dayCountNew.DOFade(1f, 0.5f).From(0).SetDelay(1f).OnComplete(() =>
                    {
                        if(FlowControllerRef.currentDay + 1 == FlowControllerRef.lastDay)
                        {
                            lastDayText.gameObject.SetActive(true);
                            lastDayText.DOFade(1f, 0.5f).From(0).SetDelay(1f);
                            lastDayText.transform.DOScale(1f, 0.5f).From(2).SetDelay(1f);
                            lastDayText.transform.DORotate(new Vector3(0, 0 , -10), 0.5f).From(Vector3.back * 360f).SetDelay(1f);
                        }
                        continuePrompt.SetActive(true);
                        continuePrompt.GetComponentInChildren<TextMeshProUGUI>().DOFade(1f, 1f).From(0).SetDelay(2.5f);
                    });
                });
            });
        });
    }

    public void EndGameBlackScreenAnimation()
    {
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Image>().DOFade(1f, 2f).SetDelay(2f).OnComplete(() =>
        {
            FlowControllerRef.EndGame();
        });
    }

    public void ContinueButton()
    {

        dayText.gameObject.SetActive(false);
        dayCountOld.gameObject.SetActive(false);
        dayCountOld.transform.localPosition = new Vector3(0f, 0f, 0f);
        dayCountNew.gameObject.SetActive(false);
        dayCountNew.transform.localPosition = new Vector3(0f, 300f, 0f);
        continuePrompt.SetActive(false);
        lastDayText.gameObject.SetActive(false);
        blackScreen.GetComponent<Image>().DOFade(0f, 2f).SetDelay(1f).OnComplete(() =>
        {
            blackScreen.SetActive(false);
            FlowControllerRef.FinishRequirement(ControllerName);
        });
    }
}
