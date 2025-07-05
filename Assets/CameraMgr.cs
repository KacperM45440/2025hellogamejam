using UnityEngine;

public class CameraMgr : Singleton<CameraMgr>
{

    public Hand hand;
    [SerializeField] private float maxCamerMoveRadius = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float cameraMoveWeightX;
    [Range(0f, 1f)]
    [SerializeField] private float cameraMoveWeightZ;

    [SerializeField] private float cameraSpeed;
    [SerializeField] private Transform cameraAnchor;
    private Camera _camera;

    public override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }


    void Update()
    {

        Vector3 dir = hand.moveTarget.position.With(y: 0) - transform.position.With(y: 0);
        dir = Vector3.Scale(dir, new Vector3(cameraMoveWeightX, 0f, cameraMoveWeightZ));
        cameraAnchor.localPosition = Vector3.Lerp(cameraAnchor.localPosition, Vector3.ClampMagnitude(dir.With(y: hand.hitFrontWall ? 1f : 0f), maxCamerMoveRadius), cameraSpeed * Time.deltaTime);

        Vector3 pos = transform.InverseTransformPoint(hand.moveTarget.position);
        float yRot = FixMinusAngle(Mathf.Clamp(pos.x, -15f, 15f));
        Quaternion rotation = Quaternion.Euler(hand.hitFrontWall ? 45f : 60f, hand.hitFrontWall ? yRot : 0f, 0f);

        //_camera.transform.localEulerAngles = Vector3.Slerp(_camera.transform.eulerAngles,
        //    new Vector3(hand.hitFrontWall ? 45f : 60f, hand.hitFrontWall ? yRot : 0f, 0f), cameraSpeed * 0.5f * Time.deltaTime);
        _camera.transform.localRotation = Quaternion.Lerp(_camera.transform.localRotation, rotation,
            cameraSpeed * 0.5f * Time.deltaTime);

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
