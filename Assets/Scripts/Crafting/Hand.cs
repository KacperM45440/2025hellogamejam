using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool active = true;
    [SerializeField] private LayerMask inputLayerMask;
    [SerializeField] private LayerMask grabLayerMask;
    [SerializeField] private LayerMask itemAnchorMask;

    public GameFlowController gameFlowController;
    public Transform moveTarget;
    public Transform rotationTarget;
    public Vector3[] rotationTargets;
    [SerializeField] private Transform socket;
    [SerializeField] private float handSpeed;
    [SerializeField] private float handOffset;
    [SerializeField] private Rigidbody handRb;
    [SerializeField] private float rotationTargetRadius = 3f;

    private bool _blockFollow = false;
    public bool hitFrontWall = false;
    private float _moveSpeed;

    public Item currentItem;
    public Item hoveredItem;
    private Item _newHoverItem;
    private ItemAnchorTarget _anchorTarget;


    [Range(0, 1)] public float lookAtWeight = 1.0f;
    public float maxWristPitch = 45f;

    private Camera _camera;
    private Vector3 _handVelocity;
    private Vector3 _previousHandPosition;

    private Sequence _grabSequence;
    private Rope _rope;

    [SerializeField] private Animator animator;
    private float _gripValue = 0;
    void Awake()
    {
        _camera = Camera.main;
        _moveSpeed = handSpeed;
    }

    void Update()
    {
        if (!active) return;
        if (_blockFollow) return;

        Controller();
        TrackHandPosition();
        RotateHand();
        _handVelocity = (moveTarget.position - _previousHandPosition) / Time.deltaTime;
        _previousHandPosition = moveTarget.position;

        if (_moveSpeed < handSpeed && !_blockFollow)
        {
            _moveSpeed += Time.deltaTime * 20f;
        }
        animator.SetFloat("Grip", _gripValue);
        
    }

    private void Controller()
    {
        if (Input.GetMouseButtonDown(0) && hoveredItem && !currentItem)
        {
            GrabItem();
        }

        if (Input.GetMouseButtonUp(0) && currentItem)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, grabLayerMask))
            {
                if (hit.transform.CompareTag("Crafting") && !CraftingMgr.Instance.currentItem)
                {
                    CraftingMgr.Instance.SetCurrentItem(currentItem);
                    DropItem(true);
                }
            }
            else if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit anchorHit, 100f,
                         itemAnchorMask))
            {
                if (anchorHit.collider.TryGetComponent(out _anchorTarget))
                {
                    if (!_anchorTarget.addedItem && _anchorTarget.itemType == currentItem.itemType)
                    {
                        _anchorTarget.parentItem.SetItemToAnchor(_anchorTarget, currentItem);
                        DropItem(true);
                    }
                    else
                    {
                        DropItem();
                    }
                }
                else
                {
                    DropItem();
                }
            }
            else
            {
                DropItem();
            }
        }
        if (Input.GetMouseButtonUp(0) && !currentItem)
        {

            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, grabLayerMask))
            {
                if (hit.collider.TryGetComponent(out _rope))
                {
                    _blockFollow = true;
                    handRb.isKinematic = true;
                    _moveSpeed = 0f;
                    handRb.transform.DOKill();        
                    //DOTween.To(() => _gripValue, x => _gripValue = x, 1f, 0.25f);
                    _gripValue = 1f;
                    animator.SetFloat("Grip", 1f);
                    handRb.transform.DOLocalRotate(new Vector3(0f, 35, -90f), 0.15f);
                    handRb.transform.DOMove(_rope.handeTarget.position + new Vector3(0.15f, 0.25f, 0f), 0.25f).OnComplete(() =>
                    {
                        handRb.DOMove(handRb.position + Vector3.down, 0.25f).OnComplete(() =>
                        {
                            moveTarget.position = handRb.position;
                            moveTarget.rotation = handRb.rotation;

                            _blockFollow = false;
                            handRb.isKinematic = false;
                            _blockFollow = false;
                            DOTween.To(() => _gripValue, x => _gripValue = x, 0f, 0.25f);

                        });
                        _rope.handeTarget.DOKill();
                        _rope.handeTarget.DOMove(_rope.handeTarget.position + Vector3.down, 0.25f).OnComplete(() =>
                        {
                            _rope.transform.DOMove(_rope.handeTarget.position - Vector3.down * 5f, 1f);
                            gameFlowController.RopeWasTugged();
                        });
                        
                    });
                   

                }
            }
        }
    }

    private void TrackHandPosition()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, inputLayerMask))
        {
            hitFrontWall = hit.transform.CompareTag("FrontWall");
            Vector3 dir = (_camera.transform.position - hit.point).normalized * handOffset;
            if (handSpeed < 0)
            {
                moveTarget.position = hit.point + dir;
            }
            else
            {
                moveTarget.position = Vector3.Lerp(moveTarget.position, hit.point + dir, _moveSpeed * Time.deltaTime);
            }
        }
    }

    private void RotateHand()
    {
        if (!hitFrontWall)
        {
            float xDiscance = moveTarget.position.x - rotationTarget.position.x;
            float zDiscance = moveTarget.position.z - rotationTarget.position.z;
            float xScale = (moveTarget.position.x > rotationTarget.position.x ? -1f : 1f) * lookAtWeight;
            float zScale = (moveTarget.position.z > rotationTarget.position.z ? 1f : -1f) * lookAtWeight;

            float angleZ = Mathf.Clamp((Mathf.Abs(xDiscance) / rotationTargetRadius), -1, 1) * maxWristPitch;
            float angleX = Mathf.Clamp((Mathf.Abs(zDiscance) / rotationTargetRadius), -1, 1) * maxWristPitch;

            Quaternion targetRotation =
                Quaternion.Euler(FixMinusAngle(angleX) * zScale, 0f, FixMinusAngle(angleZ) * xScale);
            moveTarget.rotation = Quaternion.Slerp(moveTarget.rotation, targetRotation, _moveSpeed);
        }
        else
        {
            Quaternion targetRotation =
                Quaternion.Euler(-65f, 0f, 0f);
            moveTarget.rotation = Quaternion.Slerp(moveTarget.rotation, targetRotation, _moveSpeed);
        }
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
        if (!currentItem)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, grabLayerMask))
            {
                if (hit.collider.TryGetComponent(out _newHoverItem))
                {
                    if (_newHoverItem != hoveredItem && hoveredItem)
                    {
                        hoveredItem.SetHover(false);
                    }

                    hoveredItem = _newHoverItem;
                    hoveredItem.SetHover(true);
                    _newHoverItem = null;
                }

                if (hit.collider.TryGetComponent(out _rope))
                {
                    _rope.SetOutline(true);
   
                }
                else if(_rope)
                {
                    _rope.SetOutline(false);
                }
            }
            else
            {
                if (hoveredItem)
                {
                    hoveredItem.SetHover(false);
                    hoveredItem = null;
                }

                if (_rope)
                {
                    _rope.SetOutline(false);
                }
            }
        }
        else
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, grabLayerMask))
            {
                if (hit.collider.CompareTag("Crafting") && currentItem)
                {
                    currentItem.SetOutlineColor(Color.green);
                }
                else
                {
                    currentItem.SetOutlineColor(Color.white);
                }
            }
            else if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit anchorHit, 100f,
                         itemAnchorMask))
            {
                if (anchorHit.collider.TryGetComponent(out _anchorTarget))
                {
                    currentItem.SetOutlineColor(_anchorTarget.addedItem ? Color.white : _anchorTarget.itemType == currentItem.itemType ? Color.green : Color.red);
                    //currentItem.SetOutlineColor(Color.green);
                    _anchorTarget = null;
                }
                else
                {
                    currentItem.SetOutlineColor(Color.white);
                }

            }
            else
            {
                currentItem.SetOutlineColor(Color.white);
            }

        }
    }

    private void MoveHand()
    {
        handRb.linearVelocity = (moveTarget.position - handRb.position) / Time.fixedDeltaTime;

        Quaternion rotDiff = moveTarget.rotation * Quaternion.Inverse(handRb.rotation);
        rotDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotDiffInDegree = angleInDegree * rotationAxis;
        handRb.angularVelocity = (rotDiffInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }

    private float FixMinusAngle(float angle)
    {
        if (angle < 0f)
        {
            return angle + 360f;
        }

        if (angle >= 360f)
        {
            return angle - 360f;
        }

        return angle;
    }

    public void GrabItem()
    {
        if (!hoveredItem) return;
        currentItem = hoveredItem;
        hoveredItem = null;
        handRb.isKinematic = true;
        _blockFollow = true;
        if (_grabSequence != null) _grabSequence.Kill();
        _moveSpeed = 0;

        _grabSequence = DOTween.Sequence();
        Vector3 targetHandMove = currentItem.transform.position +
                                 (socket.localPosition - currentItem.handGrabPosOffset) + Vector3.up * 0.2f;
        Vector3 backMovePosition = moveTarget.position;

        float d = 0.25f;
        _grabSequence.Insert(0f, handRb.transform.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), d / 2f));
        _grabSequence.Insert(0f, handRb.transform.DOMove(targetHandMove, d).SetEase(Ease.InCubic).OnComplete(() =>
        {
            moveTarget.position = handRb.position;
            moveTarget.rotation = handRb.rotation;
            currentItem.transform.parent = socket;
            currentItem.StartGrab();
            currentItem.DOKill();
            currentItem.transform.DOLocalMove(currentItem.handGrabPosOffset, 0.05f);
            currentItem.transform.DOLocalRotate(currentItem.handGrabRotOffset, 0.05f);
        }));
        
        DOTween.To(() => _gripValue, x => _gripValue = x, 1f, 0.25f);

        // _grabSequence.Insert(d/2f + 0.05f, handRb.transform.DOMove(backMovePosition, d/2f).OnUpdate(() =>
        //     {
        //         backMovePosition =  moveTarget.position;
        //     })
        //     .SetEase(Ease.InCubic));

        _grabSequence.OnComplete(() =>
        {
            _blockFollow = false;
            handRb.isKinematic = false;
        });
        if (CraftingMgr.Instance.currentItem == currentItem)
        {
            CraftingMgr.Instance.currentItem = null;
        }

        CraftingMgr.Instance.RefreshCollider();
    }

    public void DropItem(bool toCrafting = false)
    {
        if (!currentItem) return;
        currentItem.Drop(_handVelocity, toCrafting);

        currentItem = null;
        if (_grabSequence != null)
        {
            _grabSequence.Kill();
            if (_blockFollow) _blockFollow = false;
        }

        CraftingMgr.Instance.RefreshCollider();
        DOTween.To(() => _gripValue, x => _gripValue = x, 0f, 0.25f);

    }

    public void UpdateRotationTarget(int posIndex)
    {
        rotationTarget.DOKill();
        rotationTarget.DOLocalMove(rotationTargets[posIndex], 0.5f);
    }

    // public bool CheckCanConnectItem(Item parent, Item item, bool connect)
    // {
    //     bool finded = false;
    //     float minDist = Mathf.Infinity;
    //     ItemAnchor closestAnchor = new ItemAnchor();
    //     int index = 0;
    //     for (int i = 0; i < parent.itemAnchors.Length; i++)
    //     {
    //         if (parent.addedItems[i]) continue;
    //         if (parent.itemAnchors[i].avaliableType == item.itemType)
    //         {
    //             finded = true;
    //             float dist = Vector3.Distance(parent.itemAnchors[i].anchor.position.With(y: 0),
    //                 item.transform.position.With(y: 0));
    //             if (dist < minDist)
    //             {
    //                 minDist = dist;
    //                 closestAnchor = parent.itemAnchors[i];
    //                 index = i;
    //             }
    //         }
    //     }
    //
    //     if (finded)
    //     {
    //         if (connect)
    //         {
    //             parent.SetItemToAnchor(closestAnchor.anchor, item, index);
    //             DropItem(true);
    //         }
    //
    //         return true;
    //     }
    //
    //     return false;
    // }
}