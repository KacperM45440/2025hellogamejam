using System;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Rope : MonoBehaviour
{

    public Camera _camera;
    public Transform handeTarget;
    public SpriteRenderer outline;
    private bool isOutline = false;
    public bool finished = false;
    public Volume volume;
    public Light shoplight;
    public Light workshoplight;
    public Animator neonLogoAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rope1Sound;
    [SerializeField] private AudioClip rope2Sound;
    [SerializeField] private AudioClip changeEnviromentSound;


    private void Awake()
    {
        _camera = Camera.main;
        volume.weight = 1f;
        workshoplight.gameObject.SetActive(false);
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 dir = (_camera.transform.position.With(y: 0) - transform.position.With(y: 0)).normalized;
        transform.rotation = quaternion.LookRotation(-dir,  Vector3.up);
    }

    public void SetOutline(bool value)
    {
        if(finished) return;
        if (isOutline == value)
        {
            return;
        }

        isOutline = value;
        outline.DOKill();
        outline.DOFade(value ? 1f : 0f, 0.25f);
    }

    public void TransformStore()
    {
        StartCoroutine(TransformStoreAnimation());
    }
    
    public void PlayDropSound()
    {
        if (finished) return;
        audioSource.clip = rope1Sound;
        audioSource.Play();
    }
    
    public void PlayPullSound()
    {
        if (finished) return;
        audioSource.clip = rope2Sound;
        audioSource.Play();
    }
    
    

    private IEnumerator TransformStoreAnimation()
    {
        yield return new WaitForSeconds(1f);
        audioSource.clip = changeEnviromentSound;
        audioSource.Play();
        DOTween.To(() => volume.weight, x => volume.weight = x, 0f, 1f);
        //DOTween.To(() => light.intensity, x => light.intensity = x, 0.6f, 1f);
        Color orange = new Color(1f, 0.5f, 0f);
        DOTween.To(() => shoplight.intensity, x => shoplight.intensity = x, 5f, 1f);
        shoplight.DOColor(orange, 1f);
        workshoplight.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        neonLogoAnimator.SetTrigger("ChangeNeon");
    }

}
