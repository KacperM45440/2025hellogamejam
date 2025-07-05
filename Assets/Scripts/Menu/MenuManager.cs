using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject FadeObject;
    [SerializeField] private float duration;
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        EnableDisableButtons(true);
        FadeObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        FadeObject.GetComponent<Image>().DOFade(0, duration);

    }

    private void EnableDisableButtons(bool inOut)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = inOut;
        }
    }

    public void PlayButton()
    {
        EnableDisableButtons(false);
        StartCoroutine(FadeIN());
    }

    private IEnumerator FadeIN()
    {
        FadeObject.GetComponent<Image>().DOFade(1, duration);
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(1);
            
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Zamyka grê po zbudowaniu
#endif
    }
}
