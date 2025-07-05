using UnityEngine;
using UnityEngine.UI;

public class CloudsGenerator : MonoBehaviour
{
    [SerializeField] private Sprite[] clouds;
    [SerializeField] RectTransform Menu_RT;
    private Image image;
    private float m_MenuHeight;
    private float m_MenuWidth;

    void Start()
    {
        m_MenuHeight = Menu_RT.sizeDelta.y;
        m_MenuWidth = Menu_RT.sizeDelta.x;
        image = GetComponent<Image>();
    }

 
}
