using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public GameFlowController FlowControllerRef;
    public Hand hand;
    [SerializeField] private float maxCamerMoveRadius = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float cameraMoveWeightX;
    [Range(0f, 1f)]
    [SerializeField] private float cameraMoveWeightZ;

    [SerializeField] private float cameraSpeed;
    [SerializeField] private Transform cameraAnchor;
    private Camera _camera;
    public List<Transform> cameraPosPoints;
    public int currentPosIndex = 1;
    public bool isMoving = false;
    public Rope rope;
    public Transform tabletTransform;
    public Transform blueprintTransform;
    public bool isTablet;
    public bool isBlueprint;
    
    private bool canMoveToWorkshop = false;

    private string controllerName = "CameraController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }


    public override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }

    void Update()
    {

        Vector3 dir = hand.moveTarget.position.With(y: 0) - transform.position.With(y: 0);
        dir = Vector3.Scale(dir, new Vector3(cameraMoveWeightX, 0f, cameraMoveWeightZ));
        cameraAnchor.localPosition = Vector3.Lerp(cameraAnchor.localPosition, Vector3.ClampMagnitude(dir.With(y: hand.hitFrontWall ? (currentPosIndex == 0) ? -1f : 1f : 0f), maxCamerMoveRadius), cameraSpeed * Time.deltaTime);
 
        Vector3 pos = transform.InverseTransformPoint(hand.moveTarget.position);
        float yRot = FixMinusAngle(Mathf.Clamp(pos.x, -15f, 15f));
        Quaternion rotation = Quaternion.Euler(hand.hitFrontWall ? 45f : 60f, hand.hitFrontWall ? yRot : 0f, 0f);
        if (currentPosIndex == 0)
        {
            rotation = Quaternion.Euler(25f, yRot, 0f);
        }

        //_camera.transform.localEulerAngles = Vector3.Slerp(_camera.transform.eulerAngles,
        //    new Vector3(hand.hitFrontWall ? 45f : 60f, hand.hitFrontWall ? yRot : 0f, 0f), cameraSpeed * 0.5f * Time.deltaTime);
        _camera.transform.localRotation = Quaternion.Lerp(_camera.transform.localRotation, rotation,
            cameraSpeed * 0.5f * Time.deltaTime);


        float distanseToChange = maxCamerMoveRadius * 2f;
        float minZ = currentPosIndex == 0 ? -2.9f : -1.9f;
        if (!isMoving && hand.moveTarget.position.z <= minZ)
        {
            if (pos.x >= distanseToChange)
            {
                ChangePos(-1);
            }
            
            if (pos.x <= -distanseToChange)
            {
                ChangePos(1);
            }
        }

    }
    

    private float FixMinusAngle(float angle)
    {
        if (angle < 0f) {
            return angle + 360f;
        } 
        if (angle >= 360f) {
            return angle - 360f;
        }
        return angle;
    }

    public void UnlockWorkshop()
    {
        canMoveToWorkshop = true;
        //FlowControllerRef.FinishRequirement(controllerName);
    }

    public void ChangePos(int value)
    {
        if(isMoving) return;
        if(!canMoveToWorkshop) return;  
        int oldIndex = currentPosIndex;
        currentPosIndex -= value;
        currentPosIndex = Mathf.Clamp(currentPosIndex, 0, cameraPosPoints.Count - 1);
        if(oldIndex == currentPosIndex) return;
        isMoving = true;
        transform.DOKill(true);
        hand.UpdateRotationTarget(currentPosIndex);
        transform.DOMove(cameraPosPoints[currentPosIndex].position, 1f).OnComplete(() =>
        {
            isMoving = false;
        });
    }

    public void ShowTablet()
    {
        if(isTablet) return;
        if (isBlueprint)
        {
            HideBlueprint();
        }
        isTablet = true;
        tabletTransform.DOKill();
        tabletTransform.DOLocalMove(new Vector3(0f, 0.311f, 2.082f), 0.25f);
    }
    
    public void HideTablet()
    {
        //if(!isTablet) return;
        isTablet = false;
        tabletTransform.DOKill();
        tabletTransform.DOLocalMove(new Vector3(0f, -5f, 2.082f), 0.25f);
    }

    public void PocketTablet()
    {
        if (!isTablet) return;
        isTablet = false;
        tabletTransform.DOKill();
        tabletTransform.DOLocalMove(new Vector3(0f, -1.5f, 2.082f), 0.25f);
    }

    public void ShowBlueprint()
    {
        if (isBlueprint || isTablet) return;
        isBlueprint = true;
        blueprintTransform.DOKill();
        blueprintTransform.DOLocalMove(new Vector3(0f, -0.05f, 2.082f), 0.25f);
    }

    public void HideBlueprint()
    {
        if(!isBlueprint) return;
        isBlueprint = false;
        blueprintTransform.DOKill();
        blueprintTransform.DOLocalMove(new Vector3(0f, -5f, 2.082f), 0.25f);
    }
}
