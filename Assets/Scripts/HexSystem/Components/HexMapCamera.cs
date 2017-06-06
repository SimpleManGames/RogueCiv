using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    Transform swivel, stick;

    float zoom = 1f;

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

    public void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    public void LateUpdate()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
            AdjustZoom(zoomDelta);

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

    private void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.fixedDeltaTime;

        Vector3 position = transform.position;
        position += direction * distance;
        transform.localPosition = position;
    }
}