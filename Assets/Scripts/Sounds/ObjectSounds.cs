using System.Collections.Generic;
using UnityEngine;

public class ObjectSounds : ScriptableObject
{
    public AudioSource ObjectAudio;
    public AudioClip effect1;
    public AudioClip effect2;
    public AudioClip effect3;
    private int previousSound;
    private int i;

    public void PlaySound()
    {
        ObjectAudio.PlayOneShot(ChooseSound(), 0.5f);
    }

    private AudioClip ChooseSound()
    {
        List<AudioClip> soundList = new()
        {
            effect1,
            effect2,
            effect3,
        };

        while (i.Equals(previousSound))
        {
            i = Random.Range(0, 3);
        }

        previousSound = i;
        return soundList[i];
    }
}