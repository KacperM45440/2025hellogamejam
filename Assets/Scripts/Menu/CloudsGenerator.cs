using System.Collections;
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
    private int number = 30;

    private float m_MenuHeight;
    private float m_MenuWidth;

    void Start()
    {
        m_MenuHeight = Menu_RT.sizeDelta.y;
        m_MenuWidth = Menu_RT.sizeDelta.x;
        m_MenuHeight = 500;
        m_MenuWidth = 1000;
        GenerateFirstCloud(number);
        StartCoroutine(CloudGenerator(duration/2));
    }

    private IEnumerator CloudGenerator(float duration)
    {
        GenerateCloud();
        yield return new WaitForSeconds(duration);
        StartCoroutine(CloudGenerator(duration));
    }

    private void GenerateFirstCloud(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GenerateCloud();
        }
    }

    private void GenerateCloud()
    {
        Sprite randomCloudSprite = clouds[Random.Range(0, clouds.Length)];

        GameObject newCloud = Instantiate(cloud, new Vector2(Random.Range(-m_MenuWidth, m_MenuWidth/1.5f), Random.Range(0, m_MenuHeight)), Quaternion.identity, gameObject.transform);

        newCloud.GetComponent<Image>().sprite = randomCloudSprite;

        float randomScale = Random.Range(scaleMin, scaleMax);
        newCloud.transform.localScale = new Vector3(randomScale, randomScale, 1);
    }
}
