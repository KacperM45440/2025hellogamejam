using DG.Tweening;
using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Transform handTarget;
    public SpriteRenderer Outline;
    public bool isOutline;
    public bool interactable = true;


    public virtual void SetOutline(bool value)
    {
        if (isOutline == value)
        {
            return;
        }

        isOutline = value;
        Outline.DOKill();
        Outline.DOFade(value ? 1f : 0f, 0.25f);
    }

    public virtual void Interact()
    {
        interactable = false;
        StartCoroutine(InteractCooldown(0.5f));
    }

    private IEnumerator InteractCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        interactable = true;
    }
}
