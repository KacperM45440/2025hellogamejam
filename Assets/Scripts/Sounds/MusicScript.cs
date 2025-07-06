using System.Collections;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioClip MenuMusic;
    public AudioClip ComicMusic;
    public AudioClip GameMusic1;
    public AudioClip GameMusic2;
    public AudioClip GameMusic3;

    public void PlayMusic(string givenScene)
    {
        if (!MusicSource.isPlaying)
        {
            MusicSource.clip = ChooseMusic(givenScene);
            MusicSource.Play();
        }
    }

    public AudioClip ChooseMusic(string givenScene)
    {
        switch (givenScene)
        {
            case "menu":
                return MenuMusic;
            case "comic":
                return ComicMusic;
            case "game1":
                return GameMusic1;
            case "game2":
                return GameMusic2;
            case "game3":
                return GameMusic3;
            default:
                return null;
        }
    }

    public void DisableMusic()
    {
        MusicSource.Stop();
    }

    public void FadeMusicOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeMusicIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        while (MusicSource.volume > 0)
        {
            MusicSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FadeInRoutine()
    {
        while (MusicSource.volume < 1)
        {
            MusicSource.volume += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}