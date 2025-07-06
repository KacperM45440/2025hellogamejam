using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



public class Cloud : MonoBehaviour
{
    [SerializeField] float maxLifetime;
    [SerializeField] float minLifeTime = 5;

    [SerializeField] CanvasGroup cloud;

    float lifeTime;


    void Awake()
    {
        lifeTime = Random.Range(minLifeTime, maxLifetime);
        gameObject.transform.DOLocalMoveX((gameObject.transform.position.x +6), lifeTime * 2);

        StartCoroutine(FadeCourtine(lifeTime));
        cloud.alpha = 0;

    }
    private IEnumerator FadeCourtine(float time)
    {
        cloud.DOFade(1, time);
        yield return new WaitForSeconds(time);
        StartCoroutine(FadeCourtineDEAD(lifeTime));
    }

    private IEnumerator FadeCourtineDEAD(float time)
    {
        cloud.DOFade(0, time);
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}