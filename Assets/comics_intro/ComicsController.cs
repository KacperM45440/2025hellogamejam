using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ComicsController : MonoBehaviour
{
    [SerializeField] private Sprite[] comics; 
    [SerializeField] private Image Image;     
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

        Image.sprite = comics[currentComicIndex];

        Image.DOFade(1f, duration).OnComplete(() =>
        {
            if(currentComicIndex == comics.Length - 1)
            {
                Invoke("FadeOut", duration*3.5f);

            }
            else
            Invoke("FadeOut", duration);
        });
    }

    private void FadeOut()
    {
        Image.DOFade(0f, duration).OnComplete(() =>
        {
            currentComicIndex++;

            if (currentComicIndex < comics.Length)
            {
                ShowNextComic();
            }
            else SceneManager.LoadScene(2);

        });
    }
}
