using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    Transform swivel { get { return transform.GetChild(0); } }
    Transform stick { get { return swivel.GetChild(0); } }

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
    private HexGrid grid;

    private Vector3 Size
    {
        get
        {
            return new Vector3((grid.ChunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius), 0f, -(grid.ChunkCountZ * HexMetrics.chunkSizeZ - 1) * (1.5f * HexMetrics.outerRadius));
        }
    }

    public void Start()
    {
        AdjustZoom(0.5f);
        transform.position = Size / 2f;
        AdjustRotation(0f);
    }

    public void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
            AdjustZoom(zoomDelta);

        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
            AdjustRotation(rotationDelta);

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");

        if (xDelta != 0f || zDelta != 0f)
            AdjustPosition(xDelta, zDelta);
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
        float xMax = (grid.ChunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = -(grid.ChunkCountZ * HexMetrics.chunkSizeZ - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, zMax, 0f);

        return position;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(Size / 2f, Vector3.one);
        Gizmos.DrawWireCube(Size / 2f, Size);

        Gizmos.color = Color.green;

        Vector3 from = new Vector3(0f, 0f, stickMaxZoom);
        from = transform.position + stick.TransformVector(from);

        Vector3 to = new Vector3(0f, 0f, stickMinZoom);
        to = transform.position + stick.TransformVector(to);

        Gizmos.DrawLine(from, to);
    }
}