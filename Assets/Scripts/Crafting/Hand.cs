using System;
using DG.Tweening;
using UnityEngine;

public class Hand : MonoBehaviour
{

    public bool active = true;
    [SerializeField] private LayerMask inputLayerMask;
    [SerializeField] private LayerMask grabLayerMask;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private Transform socket;
    [SerializeField] private float handSpeed;
    [SerializeField] private float handOffset;
    [SerializeField] private Rigidbody handRb;
    [SerializeField] private float rotationTargetRadius = 3f;

    private bool _blockFollow = false;

    public Item currentItem;
    public Item hoveredItem;
    

    [Range(0, 1)]
    public float lookAtWeight = 1.0f;
    public float maxWristPitch = 45f;
    
    private RaycastHit _hit;
    private Camera _camera;

    private Sequence _grabSequence;
    
    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if(!active) return;
        if(_blockFollow) return;

        Controller();
        TrackHandPosition();
        RotateHand();
    }

    private void Controller()
    {
        if (Input.GetMouseButtonDown(0) && hoveredItem && !currentItem)
        {
            GrabItem();
        }
        
        if (Input.GetMouseButtonUp(0) && currentItem)
        {
            DropItem();
        }
    }

    private void TrackHandPosition()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out _hit, 100f,inputLayerMask))
        {
            Vector3 dir = (_camera.transform.position - _hit.point).normalized * handOffset;
            if (handSpeed < 0)
            {
                moveTarget.position = _hit.point + dir;
            }
            else
            {
                moveTarget.position = Vector3.Lerp(moveTarget.position,_hit.point + dir, handSpeed * Time.deltaTime );
            }

        }
    }

    private void RotateHand()
    {
        float xDiscance = moveTarget.position.x - rotationTarget.position.x;
        float zDiscance = moveTarget.position.z - rotationTarget.position.z;
        float xScale = (moveTarget.position.x > rotationTarget.position.x ? -1f : 1f) * lookAtWeight;
        float zScale = (moveTarget.position.z > rotationTarget.position.z ? 1f : -1f) * lookAtWeight;

        float angleZ = Mathf.Clamp((Mathf.Abs(xDiscance) / rotationTargetRadius), -1, 1) * maxWristPitch;
        float angleX = Mathf.Clamp((Mathf.Abs(zDiscance) / rotationTargetRadius), -1, 1) * maxWristPitch;

        Quaternion targetRotation = Quaternion.Euler(FixMinusAngle(angleX) * zScale, 0f, FixMinusAngle(angleZ) * xScale);
        moveTarget.rotation = targetRotation;

    }
    
    private void FixedUpdate()
    {
        if (!active) return;
        if (_blockFollow) return;
        MoveHand();
        LookingForItem();
    }
    
    private void LookingForItem()
    {
        if(currentItem) return;
        //if (Physics.Raycast(moveTarget.position, Vector3.down,  out RaycastHit hit, 10f, grabLayerMask))
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f,grabLayerMask))
        {
            if (hit.transform.TryGetComponent(out hoveredItem))
            {
                hoveredItem.SetHover(true);
            }
        }
        else
        {
            if (hoveredItem)
            {
                hoveredItem.SetHover(false);
                hoveredItem = null;
            }
        }

    }

    private void MoveHand()
    {
        handRb.linearVelocity = (moveTarget.position - handRb.position)/Time.fixedDeltaTime;

        Quaternion rotDiff = moveTarget.rotation * Quaternion.Inverse(handRb.rotation);
        rotDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotDiffInDegree = angleInDegree * rotationAxis;
        handRb.angularVelocity = (rotDiffInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
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

    public void GrabItem()
    {
        if(!hoveredItem) return;
        currentItem = hoveredItem;
        hoveredItem = null;
        handRb.isKinematic = true;
        _blockFollow = true;
        if (_grabSequence != null) _grabSequence.Kill();

        _grabSequence = DOTween.Sequence();
        Vector3 targetHandMove = currentItem.transform.position +
                                 (socket.localPosition - currentItem.handGrabPosOffset) + Vector3.up * 0.2f;

        float d = 0.25f;
        _grabSequence.Insert(0f, handRb.transform.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), d / 2f));
        _grabSequence.Insert(0f, handRb.transform.DOMove(targetHandMove, d/2f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            currentItem.transform.parent = socket;
            currentItem.StartDrag();
            currentItem.DOKill();
            currentItem.transform.DOLocalMove(currentItem.handGrabPosOffset, 0.05f);
            currentItem.transform.DOLocalRotate(currentItem.handGrabRotOffset, 0.05f);
        }));
        _grabSequence.Insert(d/2f + 0.05f, handRb.transform.DOMove(moveTarget.position, d/2f).SetEase(Ease.InCubic));

        _grabSequence.OnComplete(() =>
        {
            _blockFollow = false;
            handRb.isKinematic = false;
        
        });

    }

    public void DropItem()
    {
        if(!currentItem) return;
        currentItem.Drop();
        currentItem = null;
        if (_grabSequence != null)
        {
            _grabSequence.Kill();
            if (_blockFollow) _blockFollow = false;
        }
    }
}
