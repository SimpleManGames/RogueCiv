using UnityEngine;

public class Interaction : MonoBehaviour
{
    public HexMapCamera cameraRig;

    void Update()
    {
        DebugExtension.DebugPoint(cameraRig.Point, Color.red);
        if (Input.GetAxisRaw("Interact") != 0f)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        GetHexAt(cameraRig.Point);
    }

    private void GetHexAt(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        CubeCoord coord = FromPosition(position);
        Debug.Log(coord.ToString());
    }

    public CubeCoord FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.Instance.innerRadius * 2f);
        float y = -x;

        float offset = -position.z / (HexMetrics.Instance.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-iX - iY);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new CubeCoord(iX, iY, iZ);
    }
}