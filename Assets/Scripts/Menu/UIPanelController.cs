using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;
public class UIPanelController : MonoBehaviour
{
    [SerializeField] RectTransform PanelRectTransform;
    [SerializeField] GameObject[] Panels;

    [Header("Panel Animation")]
    private float oryginalScale = 1f;
    [SerializeField] private float startScale = 0.6f;
    [SerializeField] private float duration = 0.8f;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void SetPanel(int panel)
    {
        ResetPanelsTransforms();
        EnablePanel(panel);
        EntryAnimation();
    }

    public void ExitPanel()
    {
        ExitAnimation();
    }
    private void EntryAnimation()
    {
        PanelRectTransform.localScale = new Vector2(startScale,startScale);
        PanelRectTransform.DOScale(oryginalScale, duration).SetEase(Ease.OutBack);
    }

    private void ExitAnimation()
    {
        PanelRectTransform.DOScale(startScale, duration).SetEase(Ease.InBack);
        StartCoroutine(DisablePanel(duration));

    }
    private IEnumerator DisablePanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);

    }
    private void EnablePanel(int index)
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
            if (i == index)
            {
                Panels[index].gameObject.SetActive(true);
            }
        }
    }

    private void ResetPanelsTransforms()
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            foreach (GameObject panel in Panels)
            {
                RectTransform rectTransform = panel.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(15, 15);
                rectTransform.offsetMax = new Vector2(-15, -15);
            }


        }
    }
}
