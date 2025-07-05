using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class ButtonAnimation : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private Vector3 m_OryginalScale;
    [SerializeField] private RectTransform honey;
    [SerializeField] private Vector2 honeyStartPos;
    [SerializeField] private Vector2 honeyEnableButtonPos;

    [Header("Scale Animation")]
     public float m_HoverScale = 1.1f;
     public float m_ScaleDuration = 0.2f;

    [Header("Click Animation")]
    public float m_ClickScale = 1.1f;
    public float m_ClickDuration = 0.2f;


    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_OryginalScale = m_RectTransform.localScale;

    }

    public void OnPointerEnter()
    {
        m_RectTransform.DOScale(m_OryginalScale * m_HoverScale, m_ScaleDuration).SetEase(Ease.OutBack);
        if(honey != null)
        honey.DOAnchorPos(honeyEnableButtonPos, m_ScaleDuration*2);
    }

    public void OnPointerExit()
    {
        m_RectTransform.DOScale(m_OryginalScale, m_ScaleDuration).SetEase(Ease.OutBack);
        if (honey != null)
            honey.DOAnchorPos(honeyStartPos, m_ScaleDuration *2);
    }
}
