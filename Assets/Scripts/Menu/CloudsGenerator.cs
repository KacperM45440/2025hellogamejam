using UnityEngine;
using UnityEngine.UI;

public class CloudsGenerator : MonoBehaviour
{
    [SerializeField] private Sprite[] clouds;
    [SerializeField] RectTransform Menu_RT;
    [SerializeField] private float scaleMax = 1.3f;
    [SerializeField] private float scaleMin = 0.6f;
    [SerializeField] private GameObject cloud;
    [SerializeField] private float duration = 0.5f;

    private float m_MenuHeight;
    private float m_MenuWidth;

    void Start()
    {
        m_MenuHeight = Menu_RT.sizeDelta.y;
        m_MenuWidth = Menu_RT.sizeDelta.x;
    }

    public void CloudGenerator()
    {

    }


    private void GenerateCloud()
    {

    }

}
