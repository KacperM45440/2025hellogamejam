using DG.Tweening;
using UnityEngine;

public class Door : Singleton<Door>
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip closeDoorSound;

    private bool isOpen = false;

    [ContextMenu("Open Door")]
    public void OpenDoor()
    {
        isOpen = true;
        doorTransform.DOKill();
        doorTransform.DOLocalRotate(new Vector3(0f, 0f, openAngle), 1f).SetEase(Ease.InOutSine);
        audioSource.clip = openDoorSound;
        audioSource.Play();
    }
    
    [ContextMenu("Close Door")]
    public void CloseDoor()
    {
        isOpen = false;
        doorTransform.DOKill();
        doorTransform.DOLocalRotate(new Vector3(0f, 0f, closeAngle), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            audioSource.clip = closeDoorSound;
            audioSource.Play();
        });
    }
}
