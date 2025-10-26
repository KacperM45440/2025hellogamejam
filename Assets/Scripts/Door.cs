using DG.Tweening;
using System;
using UnityEngine;

public class Door : Singleton<Door>
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float openAngle = 120f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip closeDoorSound;

    private bool isOpen = false;

    [ContextMenu("Open Door")]
    public void OpenDoor()
    {
        isOpen = true;
        Vector3 doorRotation = doorTransform.rotation.eulerAngles;
        doorTransform.DOKill();
        doorTransform.DOLocalRotate(new Vector3(doorRotation.x, doorRotation.y, -openAngle), 1f).SetEase(Ease.InOutSine);
        audioSource.clip = openDoorSound;
        audioSource.Play();
    }
    
    [ContextMenu("Close Door")]
    public void CloseDoor()
    {
        isOpen = false;
        Vector3 doorRotation = doorTransform.rotation.eulerAngles;
        doorTransform.DOKill();
        doorTransform.DOLocalRotate(new Vector3(doorRotation.x, doorRotation.y, openAngle), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            audioSource.clip = closeDoorSound;
            audioSource.Play();
        });
    }
}
