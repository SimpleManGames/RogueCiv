using System.Linq;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    Transform swivel { get { return transform.GetChild(0); } }
    Transform stick { get { return swivel.GetChild(0); } }

    private Camera _camera;
    private Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = stick.GetChild(0).gameObject.GetComponent<Camera>();

            return _camera;
        }
    }

    float zoom = 0f;
    float rotationAngle;

    private float zoomScrollIncrements = 0.1f;

    [SerializeField]
    private float stickMinZoom;
    [SerializeField]
    private float stickMaxZoom;

    [SerializeField]
    private float swivelMinZoom;
    [SerializeField]
    private float swivelMaxZoom;

    [SerializeField]
    private float moveSpeedMinZoom;
    [SerializeField]
    private float moveSpeedMaxZoom;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    [Tooltip("That good ass slerp")]
    private float cameraElevationSpeed;

    [SerializeField]
    private HexGrid grid;

    private Vector3 Size
    {
        get
        {
            return new Vector3((grid.ChunkCountX * HexMetrics.Instance.chunkSizeX - 0.5f) * (2f * HexMetrics.Instance.innerRadius), 0f, -(grid.ChunkCountZ * HexMetrics.Instance.chunkSizeZ - 1) * (1.5f * HexMetrics.Instance.outerRadius));
        }
    }

    public void Start()
    {
        AdjustZoom(0.5f);
        transform.position = Size / 2f;
        AdjustRotation(0f);
    }

    public void LateUpdate()
    {
        float zoomDelta = Mathf.Clamp(Input.GetAxis("Zoom"), -zoomScrollIncrements, zoomScrollIncrements);
        if (zoomDelta != 0f)
            AdjustZoom(zoomDelta);

        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
            AdjustRotation(rotationDelta);

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");

        if (xDelta != 0f || zDelta != 0f)
            AdjustPosition(xDelta, zDelta);

        AdjustHeight();

        HandleCameraClip();
    }

    private void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void AdjustRotation(float delta)
    {
        rotationAngle += delta * Time.deltaTime * rotationSpeed;

        if (rotationAngle < 0f)
            rotationAngle += 360f;
        else if (rotationAngle >= 360f)
            rotationAngle -= 360f;

        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    private void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.position;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.ChunkCountX * HexMetrics.Instance.chunkSizeX - 0.5f) * (2f * HexMetrics.Instance.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = -(grid.ChunkCountZ * HexMetrics.Instance.chunkSizeZ - 1) * (1.5f * HexMetrics.Instance.outerRadius);
        position.z = Mathf.Clamp(position.z, zMax, 0f);

        return position;
    }

    private void AdjustHeight()
    {
        Vector3 groundLinecastStartPosition = Vector3.up + new Vector3(transform.position.x, stick.TransformVector(new Vector3(0f, 0f, stickMinZoom)).y, transform.position.z);
        Vector3 groundLinecastEndPosition = new Vector3(transform.position.x, 0f, transform.position.z) + Vector3.down;
        RaycastHit groundHit;

        Debug.DrawLine(groundLinecastStartPosition, groundLinecastEndPosition);

        if (Physics.Linecast(groundLinecastStartPosition, groundLinecastEndPosition, out groundHit))
        {
            DebugExtension.DebugPoint(groundHit.point);
            float difference = Mathf.Abs(transform.position.y - groundHit.point.y);
            transform.position = Vector3.Slerp(transform.position, groundHit.point, cameraElevationSpeed * difference * Time.deltaTime);
        }
    }

    private void HandleCameraClip()
    {
        Vector3[] outCorners = new Vector3[4];
        Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), Camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, outCorners);
        outCorners.ToList().ForEach(c => DebugExtension.DebugPoint(stick.TransformVector(c) + stick.position, 0.1f));

        foreach (Vector3 point in outCorners)
        {
            Debug.DrawLine(Camera.transform.position, stick.TransformVector(point) + stick.position);
            if (Physics.Linecast(stick.TransformVector(point) + stick.position, Camera.transform.position))
                AdjustZoom(-0.01f);
        }

        Debug.DrawLine(Camera.transform.position, Camera.transform.position + Camera.transform.forward * Camera.nearClipPlane);
        if (Physics.Linecast(Camera.transform.position + Camera.transform.forward * Camera.nearClipPlane, Camera.transform.position))
            AdjustZoom(-0.01f);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 from = new Vector3(0f, 0f, stickMaxZoom);
        from = transform.position + stick.TransformVector(from);

        Vector3 to = new Vector3(0f, 0f, stickMinZoom);
        to = transform.position + stick.TransformVector(to);

        Gizmos.DrawLine(from, to);

    }
}