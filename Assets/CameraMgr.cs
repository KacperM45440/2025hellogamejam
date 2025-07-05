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
        cameraAnchor.localPosition = Vector3.Lerp(cameraAnchor.localPosition, Vector3.ClampMagnitude(dir, maxCamerMoveRadius), cameraSpeed * Time.deltaTime);
    }
}
