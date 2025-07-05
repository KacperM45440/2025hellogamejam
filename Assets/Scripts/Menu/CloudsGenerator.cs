using UnityEngine;

public class CloudsGenerator : MonoBehaviour
{
    [SerializeField] RectTransform Menu_RT;
    private float m_MenuHeight;
    private float m_MenuWidth;

    void Start()
    {
        m_MenuHeight = Menu_RT.sizeDelta.y;
        m_MenuWidth = Menu_RT.sizeDelta.x;
    }

 
}
