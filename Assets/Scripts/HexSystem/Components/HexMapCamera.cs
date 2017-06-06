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
    }

    private void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
}