using DG.Tweening;
using TMPro;
using UnityEngine;

public class ClientScript : MonoBehaviour
{
    public GameObject ClientHead;
    public SpriteRenderer ClientBody;
    public SpriteRenderer Outline;
    public Animator animatorRef;
    public ClientController ClientController;
    public bool isOutline;
    public Transform gunSocket;
    public AudioSource audioSource;
    public bool outlinable = false;
    
    public void SetOutlinable(bool value)
    {
        outlinable = value;
    }

    public void SetOutline(bool value)
    {
        if (isOutline == value)
        {
            return;
        }
        if(!outlinable && value)
        {
            return;
        }

        isOutline = value;
        Outline.DOKill();
        Outline.DOFade(value ? 1f : 0f, 0.25f);
    }
    
}
