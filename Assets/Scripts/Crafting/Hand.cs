using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public bool active = true;
    [SerializeField] private LayerMask inputLayerMask;
    [SerializeField] private LayerMask grabLayerMask;
    [SerializeField] private LayerMask itemAnchorMask;
    [SerializeField] private LayerMask clientMask;

    public GameFlowController gameFlowController;
    public Transform moveTarget;
    public Transform rotationTarget;
    public Vector3[] rotationTargets;
    [SerializeField] private Transform socket;
    [SerializeField] private float handSpeed;
    [SerializeField] private float handOffset;
    [SerializeField] private Rigidbody handRb;
    [SerializeField] private float rotationTargetRadius = 3f;
    [SerializeField] private Transform itemPreviewTarget;

    private bool _blockFollow = false;
    private bool _blockFollowUI = false;
    public bool hitFrontWall = false;
    private float _moveSpeed;
    private bool _isPreviewing = false;

    public Item currentItem;
    public Item hoveredItem;
    private Item _newHoverItem;
    private ItemAnchorTarget _anchorTarget;

    public MoneyController moneyControllerRef;

    [Range(0, 1)] public float lookAtWeight = 1.0f;
    public float maxWristPitch = 45f;

    private Camera _camera;
    private Vector3 _handVelocity;
    private Vector3 _previousHandPosition;

    private DG.Tweening.Sequence _grabSequence;
    private Rope _rope;
    private InteractableObject _interactable;
    private InteractableObject hoveredInteractable;
    private ClientScript _clientScript;

    [SerializeField] private Animator animator;
    private float _gripValue = 0;
    
    private Vector3 _socketPosition;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip putSound;
    [SerializeField] private AudioClip[] mountSounds;

    void Awake()
    {
        _camera = Camera.main;
        _moveSpeed = handSpeed;
        _socketPosition = socket.localPosition;
    }

    void Update()
    {
        if (!active) return;
        if (_blockFollow || _blockFollowUI) return;

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

    public void SetUIHandBlock(bool toggle)
    {
        _blockFollowUI = toggle;
    }

    private void Controller()
    {
        if (!_isPreviewing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, clientMask | grabLayerMask))
                {
                    HandleMouseDown(hit);
                }
                else
                {
                    HandleMouseDown(null);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, grabLayerMask | itemAnchorMask))
                {
                    HandleMouseUp(hit);
                }
                else
                {
                    HandleMouseUp(null);
                }
            }
        }

        bool previousPreviewing = _isPreviewing;
        _isPreviewing = Input.GetMouseButton(1) && currentItem;

        if (_isPreviewing != previousPreviewing && currentItem)
        {
            if (_isPreviewing)
            {
                currentItem.SetOutlineColor(currentItem.itemType == ItemType.FRAME ? Color.yellow : Color.white);
                currentItem.EnableInfoText();
                socket.localPosition = _socketPosition - socket.up * 0.25f;
            }
            else
            {
                currentItem.DisableInfoText();
                _moveSpeed = 0f;
                socket.localPosition = _socketPosition;
            }
        }


    }


    private void HandleMouseDown(RaycastHit? hitInfo)
    {
        if (hoveredItem && !currentItem)
        {
            GrabItem();
            return;
        }
        else if(hoveredInteractable && !currentItem)
        {
            TryInteractWithInteractable(hitInfo);
        }

        if (!hoveredItem && !currentItem&& !hoveredInteractable)
        {
            TryProgressDialogue(hitInfo);
            return;
        }


        if (currentItem)
        {
            TryGiveItemToClient(hitInfo);
        }
    }


    private void HandleMouseUp(RaycastHit? hitInfo)
    {
        if (currentItem)
        {
            TryDropCurrentItem(hitInfo);
        }
        else
        {
            TryInteractWithRope(hitInfo);
        }
    }


    private void TryProgressDialogue(RaycastHit? hitInfo)
    {
        if (hitInfo.HasValue && ((1 << hitInfo.Value.collider.gameObject.layer) & clientMask) != 0)
        {
            if (hitInfo.Value.collider.TryGetComponent(out _clientScript))
            {
                DialogueController.Instance.ProgressDialogue();
            }
        }
    }


    private void TryGiveItemToClient(RaycastHit? hitInfo)
    {
        if (hitInfo.HasValue && ((1 << hitInfo.Value.collider.gameObject.layer) & clientMask) != 0)
        {
            if (hitInfo.Value.collider.TryGetComponent(out _clientScript))
            {
                if (_clientScript.ClientController.GetClientCanReceiveGun())
                {
                    GiveGunToClient();
                }
                else
                {
                    // ODRZUCENIE ITEMU
                }
            }
        }
        else
        {
            TryInteractWithInteractable(hitInfo);
        }
    }


    private void GiveGunToClient()
    {
        _clientScript.ClientController.ClientReceiveGun(currentItem);
        RemoveCurrentItem();
    }

    public void RemoveCurrentItem()
    {
        //GameObject gun = currentItem.gameObject;
        currentItem.SetOutlineColor(Color.clear);
        currentItem.itemPlaceholder.DOKill();
        Destroy(currentItem.itemPlaceholder.gameObject);
        CraftingMgr.Instance.currentItem = null;
        CraftingMgr.Instance.RefreshCollider();
        _gripValue = 0f;
        currentItem = null;
        //gun.transform.DOMove(_clientScript.transform.position, 0.25f);
        //gun.transform.DOScale(0f, 0.25f).OnComplete(() => { Destroy(gun); });
    }

    private void TryDropCurrentItem(RaycastHit? hitInfo)
    {
        if (!hitInfo.HasValue)
        {
            DropItem();
            return;
        }

        Transform hitTransform = hitInfo.Value.transform;

        if (hitTransform.CompareTag("Crafting") && !CraftingMgr.Instance.currentItem)
        {
            //CraftingMgr.Instance.SetCurrentItem(currentItem);
            audioSource.clip = putSound;
            audioSource.Play();
            PutItemToCrafting(CraftingMgr.Instance.craftingAnchor);
            //DropItem(true);
            return;
        }

        if (((1 << hitInfo.Value.collider.gameObject.layer) & itemAnchorMask) != 0 && hitInfo.Value.collider.TryGetComponent(out _anchorTarget))
        {
            if (!_anchorTarget.addedItem && _anchorTarget.itemType == currentItem.itemType)
            {
                audioSource.clip = mountSounds[UnityEngine.Random.Range(0, mountSounds.Length)];
                audioSource.Play();
                PutItemToCrafting(_anchorTarget.transform);
                //_anchorTarget.parentItem.SetItemToAnchor(_anchorTarget, currentItem);
                //DropItem(true);
                return;
            }
        }

        DropItem();
    }


    private void TryInteractWithRope(RaycastHit? hitInfo)
    {
        if (hitInfo.HasValue &&
            ((1 << hitInfo.Value.collider.gameObject.layer) & grabLayerMask) != 0 &&
            hitInfo.Value.collider.TryGetComponent(out _rope) && !_rope.finished)
        {
            InteractWithRope();
        }
    }


    private void InteractWithRope()
    {
        _blockFollow = true;
        handRb.isKinematic = true;
        _moveSpeed = 0f;
        handRb.transform.DOKill();
        _gripValue = 1f;
        animator.SetFloat("Grip", 1f);

        handRb.transform.DOLocalRotate(new Vector3(0f, 35, -90f), 0.15f);
        handRb.transform.DOMove(_rope.handeTarget.position + new Vector3(0.15f, 0.25f, 0f), 0.25f)
            .OnComplete(() =>
            {
                handRb.DOMove(handRb.position + Vector3.down, 0.25f).OnComplete(() =>
                {
                    moveTarget.position = handRb.position;
                    moveTarget.rotation = handRb.rotation;

                    _blockFollow = false;
                    handRb.isKinematic = false;
                    DOTween.To(() => _gripValue, x => _gripValue = x, 0f, 0.25f);
                });
                _rope.PlayPullSound();

                _rope.handeTarget.DOKill();
                _rope.handeTarget.DOMove(_rope.handeTarget.position + Vector3.down, 0.25f).OnComplete(() =>
                {
                    _rope.transform.DOMove(_rope.handeTarget.position - Vector3.down * 10f, 2f);
                    gameFlowController.RopeWasTugged();
                    _rope.finished = true;
                    _rope.TransformStore();
                });
            });
    }

    private void TryInteractWithInteractable(RaycastHit? hitInfo)
    {
        if (hitInfo.HasValue &&
            ((1 << hitInfo.Value.collider.gameObject.layer) & grabLayerMask) != 0 &&
            hitInfo.Value.collider.TryGetComponent(out _interactable) && _interactable.interactable)
        {
            InteractWithInteractable();
        }
    }


    private void InteractWithInteractable()
    {
        _blockFollow = true;
        handRb.isKinematic = true;
        _moveSpeed = 0f;
        handRb.transform.DOKill();
        _gripValue = 1f;
        animator.SetFloat("Grip", 1f);

        //handRb.transform.DOLocalRotate(new Vector3(0f, 35, -90f), 0.15f);
        handRb.transform.DOMove(_interactable.handTarget.position, 0.25f)
            .OnComplete(() =>
            {
                _interactable.Interact();

                moveTarget.position = handRb.position;
                moveTarget.rotation = handRb.rotation;

                _blockFollow = false;
                handRb.isKinematic = false;
                DOTween.To(() => _gripValue, x => _gripValue = x, 0f, 0.25f);
            });
    }


    private void TrackHandPosition()
    {
        if (!_isPreviewing)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f,
                    inputLayerMask))
            {
                hitFrontWall = hit.transform.CompareTag("FrontWall");
                Vector3 dir = (_camera.transform.position - hit.point).normalized * handOffset;
                if (handSpeed < 0)
                {
                    moveTarget.position = hit.point + dir;
                }
                else
                {
                    moveTarget.position =
                        Vector3.Lerp(moveTarget.position, hit.point + dir, _moveSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            moveTarget.position = Vector3.Lerp(moveTarget.position, itemPreviewTarget.position, _moveSpeed * Time.deltaTime);
        }
    }

    private void RotateHand()
    {
        if (!_isPreviewing)
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
        else
        {
            moveTarget.rotation = Quaternion.Slerp(moveTarget.rotation, itemPreviewTarget.rotation, _moveSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!active) return;
        if (_blockFollow || _blockFollowUI) return;
        MoveHand();
        LookingForItem();
    }

    private void LookingForItem()
    {
        if(_isPreviewing) return;
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

                    if (hoveredInteractable)
                    {
                        hoveredInteractable.SetOutline(false);
                        hoveredInteractable = null;
                    }
                }

                if (hit.collider.TryGetComponent(out _rope))
                {
                    _rope.SetOutline(true);
                }
                else if (_rope)
                {
                    _rope.SetOutline(false);
                }

                if (hit.collider.TryGetComponent(out _interactable))
                {
                    if (_interactable != hoveredInteractable && hoveredInteractable)
                    {
                        hoveredInteractable.SetOutline(false);
                    }

                    hoveredInteractable = _interactable;
                    hoveredInteractable.SetOutline(true);
                    _interactable = null;
                }
                else if (_interactable)
                {
                    _interactable.SetOutline(false);
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

                if (hoveredInteractable)
                {
                    hoveredInteractable.SetOutline(false);
                    hoveredInteractable = null;
                }
            }

            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit clientHit, 100f,
                    clientMask))
            {
                if (clientHit.collider.TryGetComponent(out _clientScript))
                {
                    _clientScript.SetOutline(true);
                }
                else if (_clientScript)
                {
                    _clientScript.SetOutline(false);
                }
            }
            else
            {
                if (_clientScript)
                {
                    _clientScript.SetOutline(false);
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
                    currentItem.SetOutlineColor(_anchorTarget.addedItem ? Color.white :
                        _anchorTarget.itemType == currentItem.itemType ? Color.green : Color.red);
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
                // if(_clientScript)
                // {
                //     _clientScript.SetOutline(false);
                // }
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
        if (hoveredItem.parentItem)
        {
            if (!hoveredItem.parentItem.inCrafting) return;
        }

        audioSource.clip = grabSound;
        audioSource.Play();
        
        currentItem = hoveredItem;
        hoveredItem = null;
        handRb.isKinematic = true;
        _blockFollow = true;
        if (_grabSequence != null) _grabSequence.Kill();
        _moveSpeed = 0;

        _grabSequence = DOTween.Sequence();
        Vector3 targetHandMove = currentItem.transform.position +
                                 (socket.localPosition - currentItem.handGrabPosOffset) + Vector3.up * 0.2f;

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
            if (CraftingMgr.Instance.currentItem)
            {
                CraftingMgr.Instance.currentItem.RefreshCircles(currentItem);
            }
        }));

        DOTween.To(() => _gripValue, x => _gripValue = x, 1f, 0.25f);

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

        if(currentItem.itemType == ItemType.MONEY)
        {
            moneyControllerRef.HandPickedUpMoney(true);
        }
    }

    public void DropItem(bool toCrafting = false)
    {
        if (!currentItem) return;
        
        currentItem.Drop(_handVelocity, toCrafting);

        if (currentItem.itemType == ItemType.MONEY)
        {
            moneyControllerRef.HandPickedUpMoney(false);
        }

        currentItem = null;
        if (_grabSequence != null)
        {
            _grabSequence.Kill();
            if (_blockFollow) _blockFollow = false;
        }

        CraftingMgr.Instance.RefreshCollider();
        if (CraftingMgr.Instance.currentItem)
        {
            CraftingMgr.Instance.currentItem.RefreshCircles();
        }

        DOTween.To(() => _gripValue, x => _gripValue = x, 0f, 0.25f);


    }

    public void PutItemToCrafting(Transform target)
    {
        if (!currentItem) return;
     
        handRb.isKinematic = true;
        _blockFollow = true;
        if (_grabSequence != null) _grabSequence.Kill();
        _moveSpeed = 0;
        
        _grabSequence = DOTween.Sequence();
        Vector3 targetHandMove = target.transform.position + Vector3.up * 0.2f;
        currentItem.itemCollider.isTrigger = true;
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

        _grabSequence.OnComplete(() =>
        {

            if (_anchorTarget)
            {
                _anchorTarget.parentItem.SetItemToAnchor(_anchorTarget, currentItem);
                _anchorTarget = null;
            }
            else
            {
                CraftingMgr.Instance.SetCurrentItem(currentItem);
            }
            DropItem(true);

            _blockFollow = false;
            handRb.isKinematic = false;
        });
        if (CraftingMgr.Instance.currentItem == currentItem)
        {
            CraftingMgr.Instance.currentItem = null;
        }

        CraftingMgr.Instance.RefreshCollider();
    }

    public void UpdateRotationTarget(int posIndex)
    {
        rotationTarget.DOKill();
        rotationTarget.DOLocalMove(rotationTargets[posIndex], 0.5f);
    }
    
}