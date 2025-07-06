using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class Rope : MonoBehaviour
{

    public Camera _camera;
    public Transform handeTarget;
    public SpriteRenderer outline;
    private bool isOutline = false;


    private void Awake()
    {
        _camera = Camera.main;
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
        if (isOutline == value)
        {
            return;
        }

        isOutline = value;
        outline.DOKill();
        outline.DOFade(value ? 1f : 0f, 0.25f);
    }
    
}
