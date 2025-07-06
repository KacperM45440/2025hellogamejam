using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class EndSceneController : MonoBehaviour
{
    //[SerializeField] private SO_WinLose sO_WinLose;
    [SerializeField] private bool win;
    [SerializeField] private Sprite winImage;
    [SerializeField] private Sprite loseImage;
    [SerializeField] private string winText = "Congratulations! You saved the Queen!";
    [SerializeField] private string loseText = "Oh... poor Queen.";

    [SerializeField] private Image Image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float duration = 7f;

    void Start()
    {
        win = Convert.ToBoolean(PlayerPrefs.GetInt("GameWin"));

        // if (sO_WinLose.win == true)
        if (win == true)
        {
            ShowNextComic(winImage , winText);
        }
        else
            ShowNextComic(loseImage, loseText);
    }

    private void ShowNextComic(Sprite image, string chooseText)
    {
        Image.sprite = image;
        text.text = chooseText;

        Image.DOFade(1f, duration * 3.5f).OnComplete(() =>
        {

                Invoke("FadeOut", duration);

  
        });
    }

    private void FadeOut()
    {
        Image.DOFade(0f, duration).OnComplete(() =>
        {
            SceneManager.LoadScene(0);

        });
    }
}
