using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class ButtonAnimation : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private Vector3 m_OryginalScale;

    [Header("Scale Animation")]
     public float m_HoverScale = 1.1f;
     public float m_ScaleDuration = 0.2f;

    [Header("Click Animation")]
    public float m_ClickScale = 1.1f;
    public float m_ClickDuration = 0.2f;


    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_RectTransform.DOScale(m_OryginalScale * m_HoverScale, m_ScaleDuration).SetEase(Ease.OutBack);
        Debug.Log("Workin1");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_RectTransform.DOScale(m_OryginalScale, m_ScaleDuration).SetEase(Ease.OutBack);
        Debug.Log("Workin2");

    }
}
