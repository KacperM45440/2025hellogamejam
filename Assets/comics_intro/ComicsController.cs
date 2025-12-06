using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ComicsController : MonoBehaviour
{
    [SerializeField] private Image[] comics;   
    [SerializeField] private float duration = 2f;  
    private int currentComicIndex = 0;  

    void Start()
    {
        if (comics.Length > 0)
        {
           Invoke("ShowNextComic",2);  
        }
    }

    private void ShowNextComic()
    {
        if (comics.Length == 0) return;

        Image currentComic = comics[currentComicIndex];
        currentComic.gameObject.SetActive(true);

        currentComic.DOFade(1f, duration).OnComplete(() =>
        {
            Invoke("FadeOut", duration);
        });
    }

    private void FadeOut()
    {
        currentComicIndex++;

        if (currentComicIndex < comics.Length)
        {
            ShowNextComic();
        }
        else
        {
            foreach(Image image in comics)
            {
                image.DOFade(0f, duration).OnComplete(() =>
                {
                    SceneManager.LoadScene(2);
                });
            }
        }
    }
}
