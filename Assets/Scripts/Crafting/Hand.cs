using System;
using UnityEngine;

public class Hand : MonoBehaviour
{

    public bool active = true;
    [SerializeField] private LayerMask inputLayerMask;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float handSpeed;
    [SerializeField] private float handOffset;
    [SerializeField] private Rigidbody handRb;
    [SerializeField] private float rotationTargetRadius = 3f;

    [Range(0, 1)]
    public float lookAtWeight = 1.0f;
    public float maxWristPitch = 45f;
    
    private RaycastHit _hit;
    private Camera _camera;
    
    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if(!active) return;

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
        
      RotateHand();
    }


    private void RotateHand()
    {
        float xDiscance = moveTarget.position.x - rotationTarget.position.x;
        float zDiscance = moveTarget.position.z - rotationTarget.position.z;
        float xScale = moveTarget.position.x > rotationTarget.position.x ? -1f : 1f;
        float zScale = moveTarget.position.z > rotationTarget.position.z ? 1f : -1f;

        float angleZ = Mathf.Clamp((Mathf.Abs(xDiscance) / rotationTargetRadius), -1, 1) * maxWristPitch;
        float angleX = Mathf.Clamp((Mathf.Abs(zDiscance) / rotationTargetRadius), -1, 1) * maxWristPitch;

        Quaternion targetRotation = Quaternion.Euler(FixMinusAngle(angleX) * zScale, 0f, FixMinusAngle(angleZ) * xScale);
        moveTarget.rotation = targetRotation;

    }

    private void FixedUpdate()
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
}
